﻿using UnityEditor;
using VisualPinball.Unity.Editor.Utils;
using VisualPinball.Unity.VPT.Rubber;

namespace VisualPinball.Unity.Editor.Inspectors
{
	[CustomEditor(typeof(RubberBehavior))]
	public class RubberInspector : DragPointsItemInspector
	{
		private RubberBehavior _rubber;
		private bool _foldoutColorsAndFormatting = true;
		private bool _foldoutPosition = true;
		private bool _foldoutPhysics = true;
		private bool _foldoutMisc = true;

		protected override void OnEnable()
		{
			base.OnEnable();
			_rubber = target as RubberBehavior;
		}

		public override void OnInspectorGUI()
		{
			base.OnPreInspectorGUI();

			if (_foldoutColorsAndFormatting = EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutColorsAndFormatting, "Colors & Formatting")) {
				DataFieldUtils.ItemDataField("Visible", ref _rubber.data.IsVisible, FinishEdit);
			}
			EditorGUILayout.EndFoldoutHeaderGroup();

			if (_foldoutPosition = EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutPosition, "Position")) {
				DataFieldUtils.ItemDataField("Height", ref _rubber.data.Height, FinishEdit);
				DataFieldUtils.ItemDataField("Thickness", ref _rubber.data.Thickness, FinishEdit);
				EditorGUILayout.LabelField("Orientation");
				EditorGUI.indentLevel++;
				DataFieldUtils.ItemDataField("RotX", ref _rubber.data.RotX, FinishEdit);
				DataFieldUtils.ItemDataField("RotY", ref _rubber.data.RotY, FinishEdit);
				DataFieldUtils.ItemDataField("RotZ", ref _rubber.data.RotZ, FinishEdit);
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndFoldoutHeaderGroup();

			if (_foldoutPhysics = EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutPhysics, "Physics")) {
				EditorGUI.BeginDisabledGroup(_rubber.data.OverwritePhysics);
				DataFieldUtils.ItemDataField("Physics Material", ref _rubber.data.PhysicsMaterial, FinishEdit, false);
				EditorGUI.EndDisabledGroup();

				DataFieldUtils.ItemDataField("Overwrite Material Settings", ref _rubber.data.OverwritePhysics, FinishEdit, false);

				EditorGUI.BeginDisabledGroup(!_rubber.data.OverwritePhysics);
				DataFieldUtils.ItemDataField("Elasticity", ref _rubber.data.Elasticity, FinishEdit, false);
				DataFieldUtils.ItemDataField("Elasticity Falloff", ref _rubber.data.ElasticityFalloff, FinishEdit, false);
				DataFieldUtils.ItemDataField("Friction", ref _rubber.data.Friction, FinishEdit, false);
				DataFieldUtils.ItemDataField("Scatter Angle", ref _rubber.data.Scatter, FinishEdit, false);
				EditorGUI.EndDisabledGroup();

				DataFieldUtils.ItemDataField("Hit Height", ref _rubber.data.HitHeight, FinishEdit, false);
				DataFieldUtils.ItemDataField("Collidable", ref _rubber.data.IsCollidable, FinishEdit, false);
				DataFieldUtils.ItemDataField("Has Hit Event", ref _rubber.data.HitEvent, FinishEdit, false);
			}
			EditorGUILayout.EndFoldoutHeaderGroup();

			if (_foldoutMisc = EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutMisc, "Misc")) {
				DataFieldUtils.ItemDataField("Timer Enabled", ref _rubber.data.IsTimerEnabled, FinishEdit, false);
				DataFieldUtils.ItemDataField("Timer Interval", ref _rubber.data.TimerInterval, FinishEdit, false);
			}
			EditorGUILayout.EndFoldoutHeaderGroup();

			base.OnInspectorGUI();
		}
	}
}
