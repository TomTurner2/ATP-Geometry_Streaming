using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class VoxelMeshGenerator : MonoBehaviour
{
    [System.Serializable]
    private enum GenerationType
    {
        CUBE,
        CUBE_CULLED,
        CUBE_GREEDY,
        MARCHING_CUBE
    };

    [SerializeField] GenerationType generation_type = GenerationType.CUBE;//so it can be set in inspector
    [SerializeField] TileManager tile_manager = null;

    static private List<Vector3> vertices = new List<Vector3>();
    static private List<Vector2> uvs = new List<Vector2>();
    static private List<int> indices = new List<int>();
    static private int face_count = 0;


    public void GenerateMesh(byte[,,] _data, ref Mesh _mesh_target)
    {
        GenerateMeshData(_data);
        UpdateMesh(ref _mesh_target);
    }


    private void GenerateMeshData(byte[,,] _data)
    {
        switch (generation_type)
        {
            case GenerationType.CUBE:
                GenerateCube(_data);
                break;
            case GenerationType.CUBE_CULLED:
                break;
            case GenerationType.CUBE_GREEDY:
                break;
            case GenerationType.MARCHING_CUBE:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    private void GenerateCube(byte[,,] _data)
    {
        byte block_id = 0;//test
        Vector2 uv = tile_manager.GetTileUV(block_id);
        Vector2 texture_unit = tile_manager.GetTextureUnit();

        CreateFaceTop(uintVector3.Zero, uv, texture_unit);
        CreateFaceBack(uintVector3.Zero, uv, texture_unit);
        CreateFaceRight(uintVector3.Zero, uv, texture_unit);
        CreateFaceFront(uintVector3.Zero, uv, texture_unit);
        CreateFaceLeft(uintVector3.Zero, uv, texture_unit);
        CreateFaceBottom(uintVector3.Zero, uv, texture_unit);
    }


    #region Top
    private void CreateFaceTop(uintVector3 _pos, Vector2 _uv, Vector2 _texture_unit)
    {
        CreateVerticesTop(_pos);
        CreateIndices();
        CreateUVs(_uv, _texture_unit);
        ++face_count;
    }


    private void CreateVerticesTop(uintVector3 _coord)
    {
        vertices.Add(new Vector3(_coord.x, _coord.y, _coord.z + 1));
        vertices.Add(new Vector3(_coord.x + 1, _coord.y, _coord.z + 1));
        vertices.Add(new Vector3(_coord.x + 1, _coord.y, _coord.z));
        vertices.Add(new Vector3(_coord.x, _coord.y, _coord.z));
    }
    #endregion


    #region Back
    private void CreateFaceBack(uintVector3 _pos, Vector2 _uv, Vector2 _texture_unit)
    {
        CreateVerticesBack(_pos);
        CreateIndices();
        CreateUVs(_uv, _texture_unit);
        ++face_count;
    }


    private void CreateVerticesBack(uintVector3 _coord)
    {
        vertices.Add(new Vector3(_coord.x + 1, _coord.y - 1, _coord.z + 1));
        vertices.Add(new Vector3(_coord.x + 1, _coord.y, _coord.z + 1));
        vertices.Add(new Vector3(_coord.x, _coord.y, _coord.z + 1));
        vertices.Add(new Vector3(_coord.x, _coord.y - 1, _coord.z + 1));
    }
    #endregion


    #region Right
    private void CreateFaceRight(uintVector3 _pos, Vector2 _uv, Vector2 _texture_unit)
    {
        CreateVerticesRight(_pos);
        CreateIndices();
        CreateUVs(_uv, _texture_unit);
        ++face_count;
    }



    private void CreateVerticesRight(uintVector3 _coord)
    {
        vertices.Add(new Vector3(_coord.x + 1, _coord.y - 1, _coord.z));
        vertices.Add(new Vector3(_coord.x + 1, _coord.y, _coord.z));
        vertices.Add(new Vector3(_coord.x + 1, _coord.y, _coord.z + 1));
        vertices.Add(new Vector3(_coord.x + 1, _coord.y - 1, _coord.z + 1));
    }
    #endregion


    #region Front
    private void CreateFaceFront(uintVector3 _pos, Vector2 _uv, Vector2 _texture_unit)
    {
        CreateVerticesFront(_pos);
        CreateIndices();
        CreateUVs(_uv, _texture_unit);
        ++face_count;
    }


    private void CreateVerticesFront(uintVector3 _coord)
    {
        vertices.Add(new Vector3(_coord.x, _coord.y - 1, _coord.z));
        vertices.Add(new Vector3(_coord.x, _coord.y, _coord.z));
        vertices.Add(new Vector3(_coord.x + 1, _coord.y, _coord.z));
        vertices.Add(new Vector3(_coord.x + 1, _coord.y - 1, _coord.z));
    }
    #endregion


    #region Left
    private void CreateFaceLeft(uintVector3 _pos, Vector2 _uv, Vector2 _texture_unit)
    {
        CreateVerticesLeft(_pos);
        CreateIndices();
        CreateUVs(_uv, _texture_unit);
        ++face_count;
    }


    private void CreateVerticesLeft(uintVector3 _coord)
    {
        vertices.Add(new Vector3(_coord.x, _coord.y - 1, _coord.z + 1));
        vertices.Add(new Vector3(_coord.x, _coord.y, _coord.z + 1));
        vertices.Add(new Vector3(_coord.x, _coord.y, _coord.z));
        vertices.Add(new Vector3(_coord.x, _coord.y - 1, _coord.z));
    }
    #endregion


    #region Bottom
    private void CreateFaceBottom(uintVector3 _pos, Vector2 _uv, Vector2 _texture_unit)
    {
        CreateVerticesBottom(_pos);
        CreateIndices();
        CreateUVs(_uv, _texture_unit);
        ++face_count;
    }


    private void CreateVerticesBottom(uintVector3 _coord)
    {
        vertices.Add(new Vector3(_coord.x, _coord.y - 1, _coord.z));
        vertices.Add(new Vector3(_coord.x + 1, _coord.y - 1, _coord.z));
        vertices.Add(new Vector3(_coord.x + 1, _coord.y - 1, _coord.z + 1));
        vertices.Add(new Vector3(_coord.x, _coord.y - 1, _coord.z + 1));
    }
    #endregion


    private void CreateIndices()
    {
        indices.Add(face_count * 4);//1
        indices.Add(face_count * 4 + 1);//2
        indices.Add(face_count * 4 + 2);//3
        indices.Add(face_count * 4);//1
        indices.Add(face_count * 4 + 2);//3
        indices.Add(face_count * 4 + 3);//4
    }


    private void CreateUVs(Vector2 _uv, Vector2 _texture_unit)
    {
        uvs.Add(new Vector2(_texture_unit.x * _uv.x + _texture_unit.x, _texture_unit.y * _uv.y));
        uvs.Add(new Vector2(_texture_unit.x * _uv.x + _texture_unit.x, _texture_unit.y * _uv.y + _texture_unit.y));
        uvs.Add(new Vector2(_texture_unit.x * _uv.x, _texture_unit.y * _uv.y + _texture_unit.y));
        uvs.Add(new Vector2(_texture_unit.x * _uv.x, _texture_unit.y * _uv.y));
    }


    private void UpdateMesh(ref Mesh _mesh)
    {
        _mesh.Clear();
        _mesh.vertices = vertices.ToArray();
        _mesh.triangles = indices.ToArray();
        _mesh.uv = uvs.ToArray();
        MeshUtility.Optimize(_mesh);
        _mesh.RecalculateNormals();

        CleanUp();
    }


    private void CleanUp()
    {
        vertices.Clear();
        indices.Clear();
        uvs.Clear();
        face_count = 0;
    }

}
