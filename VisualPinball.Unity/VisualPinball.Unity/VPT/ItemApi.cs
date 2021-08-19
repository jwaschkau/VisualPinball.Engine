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

using Unity.Entities;
using UnityEngine;
using VisualPinball.Engine.VPT;
using VisualPinball.Engine.VPT.Table;

namespace VisualPinball.Unity
{
	/// <summary>
	/// Base class for all item APIs.
	/// </summary>
	/// <typeparam name="TItem">Item type</typeparam>
	/// <typeparam name="TData">Item data type</typeparam>
	/// <typeparam name="TItemComponent">Component Type</typeparam>
	[Api]
	public abstract class ItemApi<TItemComponent, TItem, TData> : IApi
		where TItemComponent : ItemMainAuthoring<TItem, TData>
		where TItem : Item<TData>
		where TData : ItemData
	{
		/// <summary>
		/// Item name
		/// </summary>
		public string Name => MainComponent.name;

		protected readonly TItemComponent MainComponent;

		internal readonly TItem Item;
		internal readonly GameObject GameObject;

		public TData Data => Item.Data;
		private protected Table Table => _player.Table;
		private protected TableApi TableApi => _player.TableApi;

		internal VisualPinballSimulationSystemGroup SimulationSystemGroup => World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<VisualPinballSimulationSystemGroup>();

		private readonly Player _player;
		private readonly SwitchHandler _switchHandler;
		private protected BallManager BallManager;

		protected ItemApi(GameObject go, Player player)
		{
			GameObject = go;
			MainComponent = go.GetComponent<TItemComponent>();
			_player = player;
			_switchHandler = new SwitchHandler(Name, player);
		}

		private protected void OnInit(BallManager ballManager)
		{
			BallManager = ballManager;
		}

		private protected void DestroyBall(Entity ballEntity)
		{
			BallManager.DestroyEntity(ballEntity);
		}

		void IApi.OnDestroy()
		{
		}

		#region IApiSwitchable

		private protected DeviceSwitch CreateSwitch(string name, bool isPulseSwitch, SwitchDefault switchDefault = SwitchDefault.Configurable) => new DeviceSwitch(name, isPulseSwitch, switchDefault, _player);

		private protected IApiSwitchStatus AddSwitchDest(SwitchConfig switchConfig) => _switchHandler.AddSwitchDest(switchConfig);

		internal void AddWireDest(WireDestConfig wireConfig) => _switchHandler.AddWireDest(wireConfig);
		internal void RemoveWireDest(string destId) => _switchHandler.RemoveWireDest(destId);

		private protected void OnSwitch(bool closed) => _switchHandler.OnSwitch(closed);

		#endregion
	}
}
