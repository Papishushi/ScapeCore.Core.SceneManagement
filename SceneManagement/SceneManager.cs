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
 * SceneManager.cs
 * The SceneManager is responsible for managing a collection of
 * active Scene instances and provides methods to manipulate the
 * scenes. It is a crucial component used in the Scene Management
 * system facilitating the manipulation and organization of
 * scenes within the application.
 */

using ProtoBuf.Meta;
using ScapeCore.Core.Serialization.Streamers;
using ScapeCore.Core.Serialization;
using ScapeCore.Core.Tools;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;

using static ScapeCore.Core.Debug.Debugger;

namespace ScapeCore.Core.SceneManagement
{
    public class SceneManager : IScapeCoreManager
    {
        private readonly ConcurrentDictionary<int, IScene> _scenes = new();
        private int _scenesCount = 0;
        private int _currentSceneIndex = 0;
        public WeakReference<IScene?> CurrentScene { get => new(_scenes.GetValueOrDefault(_currentSceneIndex)); }
        public ImmutableList<IScene> Scenes { get => _scenes.Values.ToImmutableList(); }
        public int Count { get => _scenesCount; }
        List<IScapeCoreService?> IScapeCoreManager.Services { get => [.. _scenes.Values]; }
        private static SceneManager? _defaultManager = null;

        public static SceneManager? Default { get => _defaultManager; set => _defaultManager = value; }

        public SceneManager()
        {
            _defaultManager ??= this;
        }

        public void SetCurrentScene(int sceneIndex) => _currentSceneIndex = sceneIndex;
        public IScene? Get(int sceneId)
        {
            if (_scenes.TryGetValue(sceneId, out var scene))
                return scene;
            else
                SCLog.Log(ERROR, $"Scene with ID {sceneId} not found in the SceneManager");
            return null;
        }
        public int AddScene(IScene scene)
        {
            if (_scenesCount <= 0)
                _scenes.TryAdd(0, scene);
            else if (!_scenes.TryAdd(_scenes.Last().Key + 1, scene))
            {
                SCLog.Log(ERROR, $"There was a problem whilst trying to add Scene {scene.Name} to the SceneManager");
                return -1;
            }
            _scenesCount++;
            return _scenes.Last().Key;
        }
        public int RemoveScene(int sceneId)
        {
            if (_scenesCount <= 0)
                return -1;
            if (!_scenes.TryRemove(sceneId, out var scene))
            {
                SCLog.Log(ERROR, $"There was a problem whilst trying to remove scene {scene?.Name} to the SceneManager");
                return -1;
            }
            _scenesCount--;
            return sceneId;
        }
        public void Clear()
        {
            foreach (var scene in _scenes)
                scene.Value.Dispose();
            _scenes.Clear();
        }

        bool IScapeCoreManager.InjectDependencies(params IScapeCoreService[] services)
        {
            if (services.Length <= 0)
                return false;

            foreach(var service in services)
            {
                if (service is IScene)
                    AddScene(service as IScene);
                else return false;
            }

            return true;
        }
    }
}