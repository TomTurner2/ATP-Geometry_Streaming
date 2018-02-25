using System;
using System.Collections.Generic;
using System.Threading;


public class ThreadManager
{
    private List<Action> jobs_queue_main_thread = new List<Action>();
    private List<Thread> current_threads = new List<Thread>();
	
	//must be called from main thread
	public void Update ()
    {
        lock (jobs_queue_main_thread)//need to lock as other threads may try to use list
        {
            for (int i = 0; i < jobs_queue_main_thread.Count; ++i)
            {
                Action job = jobs_queue_main_thread[0];//get job
                if (job != null)
                    job();//call job
                jobs_queue_main_thread.RemoveAt(0);//remove job
            }
        }
	}


    public int StartThreadedJob(Action _job)
    {
        Thread thread = new Thread(new ThreadStart(_job));//create
        current_threads.Add(thread);//keep track of current threads
        thread.Start();
        return thread.ManagedThreadId;//return id reference
    }


    public bool AbortThreadedJob(int _thread_id)
    {
        for (int i = 0; i < current_threads.Count; ++i)
        {
            if (current_threads[i].ManagedThreadId != _thread_id)//check thread id
                continue;

            current_threads[i].Abort();//end the thread
            current_threads.RemoveAt(i);//remove its ref
            return true;//return true for success
        }
        return false;
    }


    public void QueueForMainThread(Action _job)
    {
        lock (jobs_queue_main_thread)
        {
            jobs_queue_main_thread.Add(_job);
        }
    }


    public void OnDestroy()
    {
        current_threads.ForEach(thread => thread.Abort());//close any lose threads
        jobs_queue_main_thread.Clear();//shouldn't really matter
    }
}
