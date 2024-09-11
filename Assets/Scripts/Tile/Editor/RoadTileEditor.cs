using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(RoadTile))]
public class RoadTileEditor : Editor
{
    /// <summary>
    /// 에디터에서 선택된 RoadTile
    /// </summary>
    RoadTile roadTIle;

    private void OnEnable()
    {
        roadTIle = target as RoadTile;      // 에디터에서 선택한 에셋을 RoadTile로 캐스팅해서 저장(캐스팅 안되면 null)
    }

    /// <summary>
    /// 에디터에서 인스펙터 창 내부를 그리는 함수
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();      // 원래 그리던 대로 그리기

        if(roadTIle != null)        // roadTile정보에 다라 추가로 그리기
        {
            if(roadTIle.sprites != null)            // 처음 만들어졌을때는 null이므로 반드시 필요
            {
                EditorGUILayout.LabelField("Sprites Image Previews");       // 제목 적기

                Texture2D texture;
                for(int i = 0; i < roadTIle.sprites.Length; i++)
                {
                    texture = AssetPreview.GetAssetPreview(roadTIle.sprites[i]);    // 스프라이트를 텍스쳐로 변경
                    if(texture != null)
                    {
                        GUILayout.Label(
                            "",     // Label의 이름(설정 안함)
                            GUILayout.Height(64),   // 높이 64
                            GUILayout.Width(64));   // 가로 64
                        GUI.DrawTexture(GUILayoutUtility.GetLastRect(), texture);   // 마지막으로 영역잡은 곳에 텍스쳐 그리기
                    }
                }
            }
        }
    }
}
#endif
