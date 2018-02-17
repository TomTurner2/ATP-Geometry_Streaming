using UnityEngine;
using System.Collections;
using System;


[Serializable]
public class Voxel
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


    const float tile_texture_size = 0.25f;//assumes 4X4 texture should change later
    public bool edited = true;


    public virtual TileTexture GetTextureCoordsByDirection(Direction _direction)//base defaults to first tile
    {
        TileTexture tile_texture = new TileTexture
        {
            x = 0,
            y = 0
        };
        return tile_texture;
    }


    public virtual Vector2[] GetFaceUVs(Direction _direction)
    {
        Vector2[] uvs = new Vector2[4];
        TileTexture tile_texture_pos = GetTextureCoordsByDirection(_direction);

        //Calculate uvs based on tile coordinates
        uvs[0] = new Vector2(tile_texture_size * tile_texture_pos.x,
            tile_texture_size * tile_texture_pos.y);
        uvs[1] = new Vector2(tile_texture_size * tile_texture_pos.x,
            tile_texture_size * tile_texture_pos.y + tile_texture_size);
        uvs[2] = new Vector2(tile_texture_size * tile_texture_pos.x + tile_texture_size,
            tile_texture_size * tile_texture_pos.y + tile_texture_size);
        uvs[3] = new Vector2(tile_texture_size * tile_texture_pos.x + tile_texture_size,
            tile_texture_size * tile_texture_pos.y);

        return uvs;
    }


    public virtual MeshInfo GetVoxelMeshInfo (Chunk _chunk, int _x, int _y, int _z, MeshInfo _mesh_info)
    {
        switch (_chunk.voxel_world.mesh_generation_type)
        {
            case MeshGenerationType.NAIVE_CUBES:
                return NaiveCube(_chunk, _x, _y, _z, _mesh_info);
            case MeshGenerationType.CUBES:
                return Cubes(_chunk, _x, _y, _z, _mesh_info);
            case MeshGenerationType.MARCHING_CUBES:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return _mesh_info;
    }


    private MeshInfo Cubes(Chunk _chunk, int _x, int _y, int _z, MeshInfo _mesh_info)
    {
        _mesh_info = CreateTopFace(_x, _y, _z, _mesh_info);
        _mesh_info = CreateBottomFace(_x, _y, _z, _mesh_info);
        _mesh_info = CreateBackFace(_x, _y, _z, _mesh_info);
        _mesh_info = CreateFrontFace(_x, _y, _z, _mesh_info);
        _mesh_info = CreateRightFace(_x, _y, _z, _mesh_info);
        _mesh_info = CreateLeftFace(_x, _y, _z, _mesh_info);

        return _mesh_info;
    }


    private MeshInfo NaiveCube(Chunk _chunk, int _x, int _y, int _z, MeshInfo _mesh_info)
    {
        //determine face to create based on adjacency
        if (ShouldMakeFace(_chunk, _x, _y + 1, _z))
            _mesh_info = CreateTopFace(_x, _y, _z, _mesh_info);

        if (ShouldMakeFace(_chunk, _x, _y - 1, _z))
            _mesh_info = CreateBottomFace(_x, _y, _z, _mesh_info);

        if (ShouldMakeFace(_chunk, _x, _y, _z + 1))
            _mesh_info = CreateBackFace(_x, _y, _z, _mesh_info);

        if (ShouldMakeFace(_chunk, _x, _y, _z - 1))
            _mesh_info = CreateFrontFace(_x, _y, _z, _mesh_info);

        if (ShouldMakeFace(_chunk, _x + 1, _y, _z))
            _mesh_info = CreateRightFace(_x, _y, _z, _mesh_info);

        if (ShouldMakeFace(_chunk, _x - 1, _y, _z))
            _mesh_info = CreateLeftFace(_x, _y, _z, _mesh_info);

        return _mesh_info;
    }


    bool ShouldMakeFace(Chunk _chunk, int _x, int _y, int _z)
    {
        Voxel voxel = _chunk.GetVoxel(_x, _y, _z);
        return (!voxel.IsSolid() || voxel.HasCustomMesh());
    }


    protected MeshInfo CreateTopFace (int _x, int _y, int _z, MeshInfo _mesh_info)
    {
        _mesh_info.AddVertex(new Vector3(_x - 0.5f, _y + 0.5f, _z + 0.5f));
        _mesh_info.AddVertex(new Vector3(_x + 0.5f, _y + 0.5f, _z + 0.5f));
        _mesh_info.AddVertex(new Vector3(_x + 0.5f, _y + 0.5f, _z - 0.5f));
        _mesh_info.AddVertex(new Vector3(_x - 0.5f, _y + 0.5f, _z - 0.5f));

        _mesh_info.AddFaceIndices();

        if (!_mesh_info.collider_only)
            _mesh_info.uvs.AddRange(GetFaceUVs(Direction.TOP));//add top uvs

        return _mesh_info;
    }


    protected MeshInfo CreateBottomFace(int _x, int _y, int _z, MeshInfo _mesh_info)
    {
        _mesh_info.AddVertex(new Vector3(_x - 0.5f, _y - 0.5f, _z - 0.5f));
        _mesh_info.AddVertex(new Vector3(_x + 0.5f, _y - 0.5f, _z - 0.5f));
        _mesh_info.AddVertex(new Vector3(_x + 0.5f, _y - 0.5f, _z + 0.5f));
        _mesh_info.AddVertex(new Vector3(_x - 0.5f, _y - 0.5f, _z + 0.5f));

        _mesh_info.AddFaceIndices();

        if (!_mesh_info.collider_only)
            _mesh_info.uvs.AddRange(GetFaceUVs(Direction.BOTTOM));

        return _mesh_info;
    }


    protected MeshInfo CreateBackFace(int _x, int _y, int _z, MeshInfo _mesh_info)
    {
        _mesh_info.AddVertex(new Vector3(_x + 0.5f, _y - 0.5f, _z + 0.5f));
        _mesh_info.AddVertex(new Vector3(_x + 0.5f, _y + 0.5f, _z + 0.5f));
        _mesh_info.AddVertex(new Vector3(_x - 0.5f, _y + 0.5f, _z + 0.5f));
        _mesh_info.AddVertex(new Vector3(_x - 0.5f, _y - 0.5f, _z + 0.5f));

        _mesh_info.AddFaceIndices();

        if (!_mesh_info.collider_only)
            _mesh_info.uvs.AddRange(GetFaceUVs(Direction.BACK));

        return _mesh_info;
    }


    protected MeshInfo CreateRightFace(int _x, int _y, int _z, MeshInfo _mesh_info)
    {
        _mesh_info.AddVertex(new Vector3(_x + 0.5f, _y - 0.5f, _z - 0.5f));
        _mesh_info.AddVertex(new Vector3(_x + 0.5f, _y + 0.5f, _z - 0.5f));
        _mesh_info.AddVertex(new Vector3(_x + 0.5f, _y + 0.5f, _z + 0.5f));
        _mesh_info.AddVertex(new Vector3(_x + 0.5f, _y - 0.5f, _z + 0.5f));

        _mesh_info.AddFaceIndices();

        if (!_mesh_info.collider_only)
            _mesh_info.uvs.AddRange(GetFaceUVs(Direction.RIGHT));

        return _mesh_info;
    }


    protected MeshInfo CreateFrontFace(int _x, int _y, int _z, MeshInfo _mesh_info)
    {
        _mesh_info.AddVertex(new Vector3(_x - 0.5f, _y - 0.5f, _z - 0.5f));
        _mesh_info.AddVertex(new Vector3(_x - 0.5f, _y + 0.5f, _z - 0.5f));
        _mesh_info.AddVertex(new Vector3(_x + 0.5f, _y + 0.5f, _z - 0.5f));
        _mesh_info.AddVertex(new Vector3(_x + 0.5f, _y - 0.5f, _z - 0.5f));

        _mesh_info.AddFaceIndices();

        if (!_mesh_info.collider_only)
            _mesh_info.uvs.AddRange(GetFaceUVs(Direction.FRONT));

        return _mesh_info;
    }


    protected MeshInfo CreateLeftFace(int _x, int _y, int _z, MeshInfo _mesh_info)
    {
        _mesh_info.AddVertex(new Vector3(_x - 0.5f, _y - 0.5f, _z + 0.5f));
        _mesh_info.AddVertex(new Vector3(_x - 0.5f, _y + 0.5f, _z + 0.5f));
        _mesh_info.AddVertex(new Vector3(_x - 0.5f, _y + 0.5f, _z - 0.5f));
        _mesh_info.AddVertex(new Vector3(_x - 0.5f, _y - 0.5f, _z - 0.5f));

        _mesh_info.AddFaceIndices();

        if (!_mesh_info.collider_only)
            _mesh_info.uvs.AddRange(GetFaceUVs(Direction.LEFT));

        return _mesh_info;
    }


    public virtual bool IsSolid()
    {
        return true;//solid by default
    }


    public virtual bool HasCustomMesh()
    {
        return false;
    }


    public virtual GameObject OnDestroy()
    {
        return null;
    }

}
