﻿// Visual Pinball Engine
// Copyright (C) 2022 freezy and VPE Team
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
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace VisualPinball.Unity.Editor
{
	public partial class AssetBrowserX
	{
		private ToolbarButton _refreshButton;
		private VisualElement _leftPane;
		private ListView _rightPane;

		public void CreateGUI()
		{
			// import UXML
			var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/org.visualpinball.engine.unity/VisualPinball.Unity/VisualPinball.Unity.Editor/AssetBrowser/AssetBrowserX.uxml");
			visualTree.CloneTree(rootVisualElement);

			var ui = rootVisualElement;

			// import style sheet
			var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/org.visualpinball.engine.unity/VisualPinball.Unity/VisualPinball.Unity.Editor/AssetBrowser/AssetBrowserX.uss");
			ui.styleSheets.Add(styleSheet);

			_leftPane = ui.Q<VisualElement>("leftPane");
			_rightPane = ui.Q<ListView>("rightPane");

			_refreshButton = ui.Q<ToolbarButton>("refreshButton");
			_refreshButton.clicked += Refresh;

			_rightPane.RegisterCallback<DragUpdatedEvent>(OnDragUpdatedEvent);
			_rightPane.RegisterCallback<DragPerformEvent>(OnDragPerformEvent);

			Init();
		}

		private void OnDestroy()
		{
			_rightPane.UnregisterCallback<DragPerformEvent>(OnDragPerformEvent);
			_rightPane.UnregisterCallback<DragUpdatedEvent>(OnDragUpdatedEvent);
			_refreshButton.clicked -= Refresh;

			foreach (var assetLibrary in _assetLibraries) {
				assetLibrary.Dispose();
			}
		}

		private void OnAssetSelectionChange(IEnumerable<object> selectedItems)
		{
			// // Clear all previous content from the pane
			// _rightPane?.Clear();
			//
			// // Get the selected asset
			// var selectedTexture = selectedItems.First() as Texture;
			// if (selectedTexture == null) {
			// 	return;
			// }
			//
			// // Add a new Image control and display the asset
			// var spriteImage = new Image {
			// 	scaleMode = ScaleMode.ScaleToFit,
			// 	image = selectedTexture
			// };
			//
			// // Add the Image control to the right-hand pane
			// _rightPane?.Add(spriteImage);
		}
	}
}