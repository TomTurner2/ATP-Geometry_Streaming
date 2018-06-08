using System;
using UnityEngine;

public static class MeshCreator
{
    public enum Direction
    {
        BACK,
        RIGHT,
        FRONT,
        LEFT,
        TOP,
        BOTTOM
    };


    public struct TileTexture
    {
        public int x;
        public int y;
    }


    const float TILE_TEXTURE_SIZE = 0.25f;//assumes 4X4 texture should change later


    public static Vector2[] GetFaceUVs(Direction _direction, Voxel _voxel)
    {
        Vector2[] uvs = new Vector2[4];
        TileTexture tile_texture_pos = _voxel.GetTextureCoordsByDirection(_direction);

        //Calculate uvs based on tile coordinates
        uvs[0] = new Vector2(TILE_TEXTURE_SIZE * tile_texture_pos.x,
            TILE_TEXTURE_SIZE * tile_texture_pos.y);
        uvs[1] = new Vector2(TILE_TEXTURE_SIZE * tile_texture_pos.x,
            TILE_TEXTURE_SIZE * tile_texture_pos.y + TILE_TEXTURE_SIZE);
        uvs[2] = new Vector2(TILE_TEXTURE_SIZE * tile_texture_pos.x + TILE_TEXTURE_SIZE,
            TILE_TEXTURE_SIZE * tile_texture_pos.y + TILE_TEXTURE_SIZE);
        uvs[3] = new Vector2(TILE_TEXTURE_SIZE * tile_texture_pos.x + TILE_TEXTURE_SIZE,
            TILE_TEXTURE_SIZE * tile_texture_pos.y);

        return uvs;
    }


    public static MeshInfo CreateMesh(Chunk _chunk, MeshInfo _mesh_info)
    {
        for (int x = 0; x < Chunk.chunk_size; ++x)
        {
            for (int y = 0; y < Chunk.chunk_size; ++y)
            {
                for (int z = 0; z < Chunk.chunk_size; ++z)
                {
                    _mesh_info = _chunk.voxels[Chunk.GetIndex(x, y, z)].GetVoxelMeshInfo(_chunk, x, y, z, _mesh_info);//Get voxels mesh info
                }
            }
        }

        return _mesh_info;
    }


    public static MeshInfo GenerateMesh(Chunk _chunk, int _x, int _y, int _z, MeshInfo _mesh_info, Voxel _voxel)
    {
        switch (_chunk.voxel_world.mesh_generation_type)
        {
            case MeshGenerationType.NAIVE_CUBES:
                return NaiveCube(_chunk, _x, _y, _z, _mesh_info, _voxel);
            case MeshGenerationType.CUBES:
                return Cubes(_chunk, _x, _y, _z, _mesh_info, _voxel);
            default:
                return _mesh_info;
        }
    }


    private static MeshInfo Cubes(Chunk _chunk, int _x, int _y, int _z, MeshInfo _mesh_info, Voxel _voxel)
    {
        _mesh_info = CreateTopFace(_x, _y, _z, _mesh_info, _voxel);
        _mesh_info = CreateBottomFace(_x, _y, _z, _mesh_info, _voxel);
        _mesh_info = CreateBackFace(_x, _y, _z, _mesh_info, _voxel);
        _mesh_info = CreateFrontFace(_x, _y, _z, _mesh_info, _voxel);
        _mesh_info = CreateRightFace(_x, _y, _z, _mesh_info, _voxel);
        _mesh_info = CreateLeftFace(_x, _y, _z, _mesh_info, _voxel);

        return _mesh_info;
    }


    private static MeshInfo NaiveCube(Chunk _chunk, int _x, int _y, int _z, MeshInfo _mesh_info, Voxel _voxel)
    {
        //determine face to create based on adjacency
        if (ShouldMakeFace(_chunk, _x, _y + 1, _z))
            _mesh_info = CreateTopFace(_x, _y, _z, _mesh_info, _voxel);

        if (ShouldMakeFace(_chunk, _x, _y - 1, _z))
            _mesh_info = CreateBottomFace(_x, _y, _z, _mesh_info, _voxel);

        if (ShouldMakeFace(_chunk, _x, _y, _z + 1))
            _mesh_info = CreateBackFace(_x, _y, _z, _mesh_info, _voxel);

        if (ShouldMakeFace(_chunk, _x, _y, _z - 1))
            _mesh_info = CreateFrontFace(_x, _y, _z, _mesh_info, _voxel);

        if (ShouldMakeFace(_chunk, _x + 1, _y, _z))
            _mesh_info = CreateRightFace(_x, _y, _z, _mesh_info, _voxel);

        if (ShouldMakeFace(_chunk, _x - 1, _y, _z))
            _mesh_info = CreateLeftFace(_x, _y, _z, _mesh_info, _voxel);

        return _mesh_info;
    }


    static bool ShouldMakeFace(Chunk _chunk, int _x, int _y, int _z)
    {
        Voxel voxel = _chunk.GetVoxel(_x, _y, _z);
        return (!voxel.IsSolid() || voxel.HasCustomMesh());
    }


    private static MeshInfo CreateTopFace(int _x, int _y, int _z, MeshInfo _mesh_info, Voxel _voxel)
    {
        _mesh_info.AddVertex(new Vector3(_x - 0.5f, _y + 0.5f, _z + 0.5f));
        _mesh_info.AddVertex(new Vector3(_x + 0.5f, _y + 0.5f, _z + 0.5f));
        _mesh_info.AddVertex(new Vector3(_x + 0.5f, _y + 0.5f, _z - 0.5f));
        _mesh_info.AddVertex(new Vector3(_x - 0.5f, _y + 0.5f, _z - 0.5f));

        _mesh_info.AddFaceIndices();

        if (!_mesh_info.collider_only)
            _mesh_info.uvs.AddRange(GetFaceUVs(Direction.TOP, _voxel));//add top uvs

        return _mesh_info;
    }


    private static MeshInfo CreateBottomFace(int _x, int _y, int _z, MeshInfo _mesh_info, Voxel _voxel)
    {
        _mesh_info.AddVertex(new Vector3(_x - 0.5f, _y - 0.5f, _z - 0.5f));
        _mesh_info.AddVertex(new Vector3(_x + 0.5f, _y - 0.5f, _z - 0.5f));
        _mesh_info.AddVertex(new Vector3(_x + 0.5f, _y - 0.5f, _z + 0.5f));
        _mesh_info.AddVertex(new Vector3(_x - 0.5f, _y - 0.5f, _z + 0.5f));

        _mesh_info.AddFaceIndices();

        if (!_mesh_info.collider_only)
            _mesh_info.uvs.AddRange(GetFaceUVs(Direction.BOTTOM, _voxel));

        return _mesh_info;
    }


    private static MeshInfo CreateBackFace(int _x, int _y, int _z, MeshInfo _mesh_info, Voxel _voxel)
    {
        _mesh_info.AddVertex(new Vector3(_x + 0.5f, _y - 0.5f, _z + 0.5f));
        _mesh_info.AddVertex(new Vector3(_x + 0.5f, _y + 0.5f, _z + 0.5f));
        _mesh_info.AddVertex(new Vector3(_x - 0.5f, _y + 0.5f, _z + 0.5f));
        _mesh_info.AddVertex(new Vector3(_x - 0.5f, _y - 0.5f, _z + 0.5f));

        _mesh_info.AddFaceIndices();

        if (!_mesh_info.collider_only)
            _mesh_info.uvs.AddRange(GetFaceUVs(Direction.BACK, _voxel));

        return _mesh_info;
    }


    private static MeshInfo CreateRightFace(int _x, int _y, int _z, MeshInfo _mesh_info, Voxel _voxel)
    {
        _mesh_info.AddVertex(new Vector3(_x + 0.5f, _y - 0.5f, _z - 0.5f));
        _mesh_info.AddVertex(new Vector3(_x + 0.5f, _y + 0.5f, _z - 0.5f));
        _mesh_info.AddVertex(new Vector3(_x + 0.5f, _y + 0.5f, _z + 0.5f));
        _mesh_info.AddVertex(new Vector3(_x + 0.5f, _y - 0.5f, _z + 0.5f));

        _mesh_info.AddFaceIndices();

        if (!_mesh_info.collider_only)
            _mesh_info.uvs.AddRange(GetFaceUVs(Direction.RIGHT, _voxel));

        return _mesh_info;
    }


    private static MeshInfo CreateFrontFace(int _x, int _y, int _z, MeshInfo _mesh_info, Voxel _voxel)
    {
        _mesh_info.AddVertex(new Vector3(_x - 0.5f, _y - 0.5f, _z - 0.5f));
        _mesh_info.AddVertex(new Vector3(_x - 0.5f, _y + 0.5f, _z - 0.5f));
        _mesh_info.AddVertex(new Vector3(_x + 0.5f, _y + 0.5f, _z - 0.5f));
        _mesh_info.AddVertex(new Vector3(_x + 0.5f, _y - 0.5f, _z - 0.5f));

        _mesh_info.AddFaceIndices();

        if (!_mesh_info.collider_only)
            _mesh_info.uvs.AddRange(GetFaceUVs(Direction.FRONT, _voxel));

        return _mesh_info;
    }


    private static MeshInfo CreateLeftFace(int _x, int _y, int _z, MeshInfo _mesh_info, Voxel _voxel)
    {
        _mesh_info.AddVertex(new Vector3(_x - 0.5f, _y - 0.5f, _z + 0.5f));
        _mesh_info.AddVertex(new Vector3(_x - 0.5f, _y + 0.5f, _z + 0.5f));
        _mesh_info.AddVertex(new Vector3(_x - 0.5f, _y + 0.5f, _z - 0.5f));
        _mesh_info.AddVertex(new Vector3(_x - 0.5f, _y - 0.5f, _z - 0.5f));

        _mesh_info.AddFaceIndices();

        if (!_mesh_info.collider_only)
            _mesh_info.uvs.AddRange(GetFaceUVs(Direction.LEFT, _voxel));

        return _mesh_info;
    }

}
