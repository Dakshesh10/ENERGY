using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CodeAlchemy.CommonUtils
{
    [CustomEditor(typeof(GridManager))]
    public class GridManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GridManager gridManager = (GridManager)target;

            if (DrawDefaultInspector())
            {
                //if(gridManager.autoUpdate)
                //{
                //    gridManager.OnGenerateClicked();
                //    gridManager.gameObject.name = string.Format("Grid_{0}X{1}", gridManager.size.x, gridManager.size.z);
                //}
            }

            if (GUILayout.Button("Generate"))
            {
                gridManager.OnGenerateClicked();
                gridManager.gameObject.name = string.Format("Grid_{0}X{1}", gridManager.size.x, gridManager.size.y);
            }
        }

        [MenuItem("CodeAlchemy/Create Grid")]
        public static void AddGridManagerObj()
        {
            GameObject newGrid = new GameObject("Grid", typeof(GridManager));
            GridManager gridManager = newGrid.GetComponent<GridManager>();
            gridManager.cellScale = new Vector2(5, 5);
            //gridManager.autoUpdate = true;
            gridManager.OnGenerateClicked();
            gridManager.gameObject.name = string.Format("Grid_{0}X{1}", gridManager.size.x, gridManager.size.y);

            Selection.activeGameObject = newGrid;
        }
    }
}
