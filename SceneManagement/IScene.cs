/*
 * -*- encoding: utf-8 with BOM -*-
 * .▄▄ ·  ▄▄·  ▄▄▄·  ▄▄▄·▄▄▄ .     ▄▄·       ▄▄▄  ▄▄▄ .
 * ▐█ ▀. ▐█ ▌▪▐█ ▀█ ▐█ ▄█▀▄.▀·    ▐█ ▌▪▪     ▀▄ █·▀▄.▀·
 * ▄▀▀▀█▄██ ▄▄▄█▀▀█  ██▀·▐▀▀▪▄    ██ ▄▄ ▄█▀▄ ▐▀▀▄ ▐▀▀▪▄
 * ▐█▄▪▐█▐███▌▐█ ▪▐▌▐█▪·•▐█▄▄▌    ▐███▌▐█▌.▐▌▐█•█▌▐█▄▄▌
 *  ▀▀▀▀ ·▀▀▀  ▀  ▀ .▀    ▀▀▀     ·▀▀▀  ▀█▄▀▪.▀  ▀ ▀▀▀ 
 * https://github.com/Papishushi/ScapeCore
 * 
 * Copyright (c) 2023 Daniel Molinero Lucas
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 * 
 * Scene.cs
 * Represents an environment containing a collection of active behaviours, exposes
 * multiple methods to manipulate the scene. This class is mainly used in the
 * Sceme Management system.
 */

using ScapeCore.Core.Tools;
using ScapeCore.Core.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScapeCore.Core.SceneManagement
{
    public interface IScene : IDisposable, IScapeCoreService
    {
        string Name { get; set; }

        IList GameObjects { get; }
        IList MonoBehaviours { get; }

        DeeplyMutableType AddToScene(Type type);
        T? AddToScene<T>() where T : MonoBehaviour;
        Task<DeeplyMutableType> AddToSceneAsync(Type type);
        Task<T?> AddToSceneAsync<T>() where T : MonoBehaviour;
        Task<List<T>> AddToSceneMultipleAsync<T>(int cuantity) where T : MonoBehaviour;
        void Dispose();
        IEnumerator<GameObject>? FindGameObjectsWithTag(string tag);
        GameObject? FindGameObjectWithTag(string tag);
        void RemoveFromScene(GameObject gameObject);
        void RemoveFromScene(MonoBehaviour monoBehaviour);
    }
}