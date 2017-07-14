﻿using System;
using System.Collections.Generic;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace PrepareLanding.Gui.World
{
    public class HighlightedTileDrawer : IDisposable
    {
        public const int MaxHighlightedTiles = 10000;

        private const float MaxDistToCameraToDisplayLabel = 39f;

        private readonly List<HighlightedTile> _highlightedTiles = new List<HighlightedTile>();

        private readonly Material _defaultMaterial = new Material(WorldMaterials.SelectedTile);

        private Color _materialColor = Color.green;

        public Color TileColor
        {
            get { return _materialColor; }
            set
            {
                _materialColor = value;
                _defaultMaterial.color = _materialColor;
            }
        }

        public bool ShowDebugTileId { get; set; }

        public bool BypassMaxHighlightedTiles { get; set; }

        public HighlightedTileDrawer()
        {
            PrepareLanding.Instance.OnWorldInterfaceOnGui += HighlightedTileDrawerOnGui;
            PrepareLanding.Instance.OnWorldInterfaceUpdate += HighlightedTileDrawerUpdate;

            _defaultMaterial.color = TileColor;
        }

        internal void HighlightTile(int tile, float colorPct = 0f, string text = null)
        {
            var highlightedTile = new HighlightedTile(tile, colorPct, text);
            _highlightedTiles.Add(highlightedTile);
        }

        public void HighlightTileList(List<int> tileList)
        {
            // do not highlight too many tiles (otherwise the slow down is noticeable)
            if (!BypassMaxHighlightedTiles && tileList.Count > MaxHighlightedTiles)
            {
                PrepareLanding.Instance.TileFilter.FilterInfo.AppendErrorMessage($"Too many tiles to highlight ({tileList.Count}). Try to add more filters to decrease the actual count.");
                return;
            }

            foreach (var tileId in tileList)
            {
                HighlightTile(tileId, _defaultMaterial, ShowDebugTileId ? tileId.ToString() : "X");
            }
        }

        public void HighlightTile(int tile, Material mat, string text = null)
        {
            var debugTile = new HighlightedTile(tile, mat, text);
            _highlightedTiles.Add(debugTile);
        }

        public void RemoveAllTiles()
        {
            _highlightedTiles.Clear();
        }

        protected void HighlightedTileDrawerUpdate()
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < _highlightedTiles.Count; i++)
            {
                _highlightedTiles[i].Draw();
            }
        }

        protected void HighlightedTileDrawerOnGui()
        {
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.MiddleCenter;
            GUI.color = new Color(1f, 1f, 1f, 0.5f);
            foreach (var tile in _highlightedTiles)
            {
                if (tile.DistanceToCamera <= MaxDistToCameraToDisplayLabel)
                {
                    tile.OnGui();
                }
            }
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        #region IDisposable Support
        private bool _disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue)
                return;

            if (disposing)
            {
                _highlightedTiles.Clear();

                PrepareLanding.Instance.OnWorldInterfaceOnGui -= HighlightedTileDrawerOnGui;
                PrepareLanding.Instance.OnWorldInterfaceUpdate -= HighlightedTileDrawerUpdate;
            }

            // TODO: set large fields to null.

            _disposedValue = true;
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
