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

using System;
using System.Collections.Generic;
using UnityEngine;
using VisualPinball.Engine.Game;
using VisualPinball.Engine.VPT.Flipper;
using VisualPinball.Engine.VPT.Table;

namespace VisualPinball.Unity
{
	[ExecuteInEditMode]
	[AddComponentMenu("Visual Pinball/Mesh/Flipper Rubber Mesh")]
	public class FlipperRubberMeshAuthoring : ItemMeshAuthoring<FlipperData, FlipperAuthoring>
	{
		public static readonly Type[] ValidParentTypes = Type.EmptyTypes;

		public override IEnumerable<Type> ValidParents => ValidParentTypes;

		protected override string MeshId => FlipperMeshGenerator.Rubber;

		protected override RenderObject GetRenderObject(FlipperData data)
		{
			return new FlipperMeshGenerator(data).GetRenderObject(table, FlipperMeshGenerator.Rubber, Origin.Original, false);
		}
	}
}
