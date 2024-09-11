using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RoadTile : Tile
{
    [Flags]
    enum AdjTilePosition : byte
    {
        None = 0,                               // 0000 0000
        North = 1,                              // 0000 0001
        East = 2,                               // 0000 0010
        South = 4,                              // 0000 0100
        West = 8,                               // 0000 1000
        All = North | East | South | West       // 0000 1111
    }

    /// <summary>
    /// 길을 구성하는 스프라이트들
    /// </summary>
    public Sprite[] sprites;

    /// <summary>
    /// 타일이 그려질 때 자동으로 호출되는 함수
    /// </summary>
    /// <param name="position">타일의 위치(그리드 좌표)</param>
    /// <param name="tilemap">이 타일이 그려지는 타일맵</param>
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        // 주변에 있는 같은 종류의 타일 갱신하기

        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                Vector3Int location = new Vector3Int(position.x + x, position.y + y, position.z);   // position 주변 위치 구하기
                if(HasThisTile(tilemap, location))      // 주변 위치가 같은 타일이면
                {
                    tilemap.RefreshTile(location);      // 같은 타일일때만 갱신
                }
            }
        }
    }

    /// <summary>
    /// 타일맵의 RefreshTile함수가 호출될 때 호출, 타일이 어떤 스프라이트를 그릴지 결정하는 함수
    /// </summary>
    /// <param name="position">타일 데이터를 가져올 타일의 위치</param>
    /// <param name="tilemap">타일 데이터를 가져올 타일맵</param>
    /// <param name="tileData">가져온 타일 데이터의 참조(읽기쓰기 둘 다 가능)</param>
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        // 주변 타일정보 확인해서 스프라이트 선택

        AdjTilePosition mask = AdjTilePosition.None;

        //if(HasThisTile(tilemap, position + new Vector3Int(0,1,0)))
        //{
        //    mask = mask | AdjTilePosition.North;
        //}

        // (결과) = (조건) ? (참일때의 값) : (거짓일때의 값);  // 삼항연산자

        mask |= HasThisTile(tilemap, position + new Vector3Int(0, 1, 0)) ? AdjTilePosition.North : AdjTilePosition.None;
        mask |= HasThisTile(tilemap, position + new Vector3Int(1, 0, 0)) ? AdjTilePosition.East : AdjTilePosition.None;
        mask |= HasThisTile(tilemap, position + new Vector3Int(0, -1, 0)) ? AdjTilePosition.South : AdjTilePosition.None;
        mask |= HasThisTile(tilemap, position + new Vector3Int(-1, 0, 0)) ? AdjTilePosition.West : AdjTilePosition.None;

        int index = GetIndex(mask);                 // 인덱스 결정
        if(index > -1 && index < sprites.Length)
        {
            tileData.sprite = sprites[index];       // 스프라이트 결정
            Matrix4x4 matrix = tileData.transform;
            matrix.SetTRS(Vector3.zero, GetRotation(mask), Vector3.one);    // 행렬에 회전만 설정
            tileData.transform = matrix;
            tileData.flags = TileFlags.LockTransform;   // 다른 타일이 회전을 못시키게 잠금
        }
        else
        {
            Debug.LogError($"잘못된 인덱스 : {index}, mask = {mask}");
        }
    }
    
    /// <summary>
    /// 특정 타일맵의 특정 위치에 이 타일과 같은 종류의 타일이 있는지 확인하는 함수
    /// </summary>
    /// <param name="tilemap">확인할 타일맵</param>
    /// <param name="position">확인할 위치</param>
    /// <returns>true면 같은 종류의 타일, false면 다른 종류의 타일</returns>
    bool HasThisTile(ITilemap tilemap, Vector3Int position)
    {
        return tilemap.GetTile(position) == this;       // 같은 객체인지 확인
    }

    /// <summary>
    /// 마스크값에 따라 그릴 스프라이트의 인덱스를 리턴하는 함수
    /// </summary>
    /// <param name="mask"></param>
    /// <returns></returns>
    int GetIndex(AdjTilePosition mask)
    {
        int index = -1;

        switch(mask)
        {
            case AdjTilePosition.West | AdjTilePosition.South:  // 남서
            case AdjTilePosition.North | AdjTilePosition.West:  // 북서
            case AdjTilePosition.North | AdjTilePosition.East:  // 북동
            case AdjTilePosition.East | AdjTilePosition.South:  // 남동
                index = 1;  // ㄱ자 모양
                break;
            case AdjTilePosition.All & ~AdjTilePosition.North:  // (0000 1111) & ~(0000 0001) = (0000 1111) & (0000 1110) = 0000 1110 = 북쪽만 뺀 전부
            case AdjTilePosition.All & ~AdjTilePosition.East:
            case AdjTilePosition.All & ~AdjTilePosition.South:
            case AdjTilePosition.All & ~AdjTilePosition.West:
                index = 2;  // ㅗ자 모양
                break;
            case AdjTilePosition.All:
                index = 3;  // +자 모양
                break;
            case AdjTilePosition.None:      // 주변에 없음
            case AdjTilePosition.North:     // 북쪽에만 있음
            case AdjTilePosition.East:      // 동쪽에만 있음
            case AdjTilePosition.South:     // 남쪽에만 있음
            case AdjTilePosition.West:      // 서쪽에만 있음
            case AdjTilePosition.North | AdjTilePosition.South:     // 북쪽과 남쪽에 있음
            case AdjTilePosition.East | AdjTilePosition.West:       // 동쪽과 서쪽에 있음
            default:
                index = 0;
                break;
        }

        return index;
    }

    /// <summary>
    /// 마스크 값에 따라 스프라이트를 얼마나 회전시킬 것인지 결정하는 함수
    /// </summary>
    /// <param name="mask">주변 타일 상황을 기록해 놓은 마스크</param>
    /// <returns>최종 회전</returns>
    Quaternion GetRotation(AdjTilePosition mask)
    {
        Quaternion rotate = Quaternion.identity;

        switch(mask)
        {
            case AdjTilePosition.East:
            case AdjTilePosition.West:
            case AdjTilePosition.East | AdjTilePosition.West:       // 1자
            case AdjTilePosition.North | AdjTilePosition.West:      // ㄱ자
            case AdjTilePosition.All & ~AdjTilePosition.West:        // ㅗ자
                rotate = Quaternion.Euler(0, 0, -90);
                break;
            case AdjTilePosition.North | AdjTilePosition.East:       // ㄱ자
            case AdjTilePosition.All & ~AdjTilePosition.North:       // ㅗ자
                rotate = Quaternion.Euler(0, 0, -180);
                break;
            case AdjTilePosition.South | AdjTilePosition.East:      // ㄱ자
            case AdjTilePosition.All & ~AdjTilePosition.East:       // ㅗ자
                rotate = Quaternion.Euler(0, 0, -270);
                break;
        }

        return rotate;
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/2D/Tiles/Custom/RoadTile")]    // 메뉴추가
    public static void CreateRoadTile() // 메뉴가 클릭되었을 때 실행될 함수(이름은 상관없음)
    {
        // 창을 열어서 풀 경로 받기
        string path = EditorUtility.SaveFilePanelInProject(
            "Save Road Tile",   // 제목
            "New Road Tile",    // 기본 파일명
            "Asset",            // 기본 확장자
            "Save Road Tile",   // 출력메세지
            "Assets/Tiles"      // 열릴 기본 로더
            );

        if(path != string.Empty)        // 경로를 받았으면
        {
            AssetDatabase.CreateAsset(CreateInstance<RoadTile>(), path);
        }
    }
#endif
}
