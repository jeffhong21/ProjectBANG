// =============================================================================
// MIT License
// 
// Copyright (c) 2018 Valeriya Pudova (hww.github.io)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// =============================================================================

#if UNITY_EDITOR
using System;
using UnityEditor;
#endif
using System;
using UnityEngine;

namespace VARP.VisibilityEditor
{
    public static class ArtLayers
    {
        private const string applyLayersColorsParamName = "ArtLayers.applyLayersColors";
        public static readonly ArtLayer[] Layers = new ArtLayer[32];
        public static bool applyLayersColors;

        static ArtLayers()
        {
            // -- initialize all layers --
            var layersValues = Enum.GetValues(typeof(GameLayer));
            foreach (var layer in layersValues)
                Layers[(int) layer] = new ArtLayer((int) layer, ((GameLayer) layer).ToString(), Color.white);
        }

        public static bool ApplyColors
        {
            get
            {
#if UNITY_EDITOR
                return EditorPrefs.GetBool(applyLayersColorsParamName);
#else
                return false;
#endif
            }
            set
            {
#if UNITY_EDITOR
                applyLayersColors = value;
                EditorPrefs.SetBool(applyLayersColorsParamName, applyLayersColors);
#else
#endif
            }
        }

        /// <summary>
        ///     Get art layer by tag
        /// </summary>
        /// <param name="gameLayer"></param>
        /// <returns></returns>
        public static ArtLayer GetLayer(GameLayer gameLayer)
        {
            return Layers[(int) gameLayer];
        }

        /// <summary>
        ///     Get art layer by integer index (same at is in Unity)
        /// </summary>
        /// <param name="gameLayer"></param>
        /// <returns></returns>
        public static ArtLayer GetLayer(int gameLayer)
        {
            return Layers[gameLayer];
        }
    }
}