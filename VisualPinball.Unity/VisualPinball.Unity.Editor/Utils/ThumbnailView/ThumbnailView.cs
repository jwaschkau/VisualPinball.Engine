﻿// Visual Pinball Engine
// Copyright (C) 2021 freezy and VPE Team
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

namespace VisualPinball.Unity.Editor
{
	/// <summary>
	/// 
	/// </summary>
	public class ThumbnailViewStyles
	{
		public GUIStyle DefaultStyle;
		public GUIStyle SelectedStyle;
		public GUIStyle NameStyle;
		public GUIStyle HoverStyle;
		public bool Inited => DefaultStyle != null && SelectedStyle != null && NameStyle != null && HoverStyle != null;
	}

	/// <summary>
	/// A thumbnail view which can be integrated in any Unity GUI layout
	/// It manages <see cref="ThumbnailElement"/> on a planar view.
	/// It'll use the custom <see cref="ThumbnailElement"/> draw methods to display each element in its own rect.
	/// </summary>
	/// <typeparam name="T">a <see cref="ThumbnailElement"/> generic class</typeparam>
	public abstract class ThumbnailView<T> where T : ThumbnailElement
	{
		public struct RowCollums
		{
			public int Rows;
			public int Collumns;

			public bool Valid => Rows > 0 && Collumns > 0;
		}

		private List<T> _data = new List<T>();

		private Vector2 _scroll;

		private List<T> _selectedItems = new List<T>();
		public List<T> SelectedItems => _selectedItems;
		public T SelectedItem => _selectedItems.Count == 1 ? _selectedItems[0] : null;

		private string _searchFilter = string.Empty;

		public bool ShowToolbar = true;

		public bool MultiSelection = false;

		protected EThumbnailSize _thumbnailSize = EThumbnailSize.Normal;

		protected ThumbnailViewStyles _commonStyles = new ThumbnailViewStyles();

		public ThumbnailView(IEnumerable<T> data)
		{
			SetData(data);
		}

		public void SetData(IEnumerable<T> data)
        {
			_data.Clear();
			if (data != null) {
				_data.AddRange(data);
            }
        }

		protected abstract void InitCommonStyles();

		protected virtual void OnGUIToolbar() { }

		protected virtual bool MatchLabelFilter(T item, string labelFilter) => false;


		private static int FilterCompare(string f1, string f2)
		{
			var f1Label = f1.StartsWith("l:", StringComparison.InvariantCultureIgnoreCase);
			var f2Label = f2.StartsWith("l:", StringComparison.InvariantCultureIgnoreCase);
			if (f1Label != f2Label)
				return f1Label ? 1 : -1;
			return f1.CompareTo(f2);
		}

		private List<T> GetFilteredItems()
		{
			List<T> filteredNames = new List<T>();
			List<T> filteredLabels = new List<T>();
			filteredNames.AddRange(_data);
			var filters = _searchFilter.Split(' ').ToList();
			filters.Sort(FilterCompare);
			foreach (var filter in filters) {
				if (string.IsNullOrEmpty(filter)) continue;
				if (filter.StartsWith("l:", StringComparison.InvariantCultureIgnoreCase)) {
					var labelFilter = filter.Split(':')[1];
					if (!string.IsNullOrEmpty(labelFilter)) {
						filteredLabels = filteredLabels.Union(_data.Where(I => MatchLabelFilter(I, labelFilter))).ToList();
					}
				} else {
					filteredNames = filteredNames.Where(I => I.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
				}
			}
			if (filteredLabels.Count == 0)
				return filteredNames;
			return filteredNames.Intersect(filteredLabels).ToList();
		}

		public void OnGUI(Rect rect)
        {
			InitCommonStyles();

			EditorGUILayout.BeginVertical();

			if (ShowToolbar) {
				EditorGUILayout.BeginVertical();

				EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(rect.width));
				EditorGUILayout.LabelField("Name Filter", GUILayout.Width(100));
				_searchFilter = EditorGUILayout.TextField(_searchFilter, EditorStyles.toolbarSearchField, GUILayout.Width(rect.width-320));

				var buttonRect = EditorGUILayout.GetControlRect(GUILayout.Width(GUI.skin.label.lineHeight));
				buttonRect.width = GUI.skin.label.lineHeight;
				if ((GUI.Button(buttonRect, GUIContent.none, !string.IsNullOrEmpty(_searchFilter) ? "ToolbarSeachCancelButton" : "ToolbarSeachCancelButtonEmpty") && !string.IsNullOrEmpty(_searchFilter))){
					_searchFilter = string.Empty;
					GUIUtility.keyboardControl = 0;
				}

				EditorGUILayout.LabelField("Thumbnail Size", GUILayout.Width(100));
				_thumbnailSize = (EThumbnailSize)EditorGUILayout.EnumPopup(_thumbnailSize, GUILayout.Width(100));
				EditorGUILayout.EndHorizontal();

				OnGUIToolbar();

				EditorGUILayout.EndVertical();
			}

			var filteredItems = GetFilteredItems(); 

			_selectedItems = filteredItems.Intersect(_selectedItems).ToList();

			if (filteredItems.Count > 0) {
				var buttonRect = filteredItems[0].CommonSizes[_thumbnailSize];
				var rowcol = ComputeRowCollums(rect.width - buttonRect.x * 2.0f);

				if (rowcol.Valid) {
					Rect fullRect = EditorGUILayout.GetControlRect(false, ((buttonRect.y + buttonRect.height + _commonStyles.NameStyle.lineHeight) * rowcol.Rows) + buttonRect.y, GUILayout.Width(rect.width - GUI.skin.box.padding.left));
					Rect viewRect = new Rect(fullRect.x, fullRect.y, fullRect.width + 15, rect.height - fullRect.y);

					_scroll = GUI.BeginScrollView(viewRect, _scroll, fullRect, false, true);
					Rect scrolledViewRect = new Rect(viewRect.position + _scroll, viewRect.size);

					for (int i = 0; i < filteredItems.Count; i++) {
						var item = filteredItems[i];
						var col = i % rowcol.Collumns;
						var row = i / rowcol.Collumns;
						Rect itemRect = new Rect(fullRect.x + col * (buttonRect.x + buttonRect.width) + buttonRect.x, fullRect.y + row * (buttonRect.y + buttonRect.height + _commonStyles.NameStyle.lineHeight) + buttonRect.y, buttonRect.width, buttonRect.height);

						HandleEvents(item, itemRect);

						if (itemRect.Overlaps(scrolledViewRect)) {
							item.OnGUI(itemRect, _selectedItems.Contains(item) ? _commonStyles.SelectedStyle : _commonStyles.DefaultStyle);
							if (!string.IsNullOrEmpty(item.Name)) {
								itemRect.y += itemRect.height;
								itemRect.height = _commonStyles.NameStyle.lineHeight;
								GUI.Label(itemRect, item.Name, _commonStyles.NameStyle);
							}
						}
					}

					GUI.EndScrollView();
				}
			}

			EditorGUILayout.EndVertical();
		}

		public void HandleEvents(T item, Rect itemRect)
		{
			var evt = Event.current;

			switch(evt.type) {

				case EventType.MouseDown: {
					if (evt.button == 0 && itemRect.Contains(evt.mousePosition)) {
						if (evt.control && MultiSelection) {
							if (!_selectedItems.Contains(item))
								_selectedItems.Add(item);
							else
								_selectedItems.Remove(item);
						} else {
							_selectedItems.Clear();
							_selectedItems.Add(item);
						}
						evt.Use();
					}
					break;
				}
			}

		}

		public RowCollums ComputeRowCollums(float viewWidth)
		{
			if (_data.Count == 0 || viewWidth <= 0.0f)
				return new RowCollums() { Rows = 0, Collumns = 0 };

			var buttonRect = _data[0].CommonSizes[_thumbnailSize];
			int nbCollumns = (int)((viewWidth - 5) / (buttonRect.width + buttonRect.x));
			if (nbCollumns == 0)
				return new RowCollums() { Rows = 0, Collumns = 0 };
			int nbRows = _data.Count / nbCollumns;
			if (_data.Count % nbCollumns != 0) {
				nbRows++;
			}
			return new RowCollums() { Rows = nbRows, Collumns = nbCollumns };
		}

		public float ComputeViewHeight(float viewWidth)
		{
			if (_data.Count == 0)
				return 0.0f;
			var buttonRect = _data[0].CommonSizes[_thumbnailSize];
			var rowcol = ComputeRowCollums(viewWidth);
			return (buttonRect.y + buttonRect.height) * rowcol.Rows;
		}
	}
}
