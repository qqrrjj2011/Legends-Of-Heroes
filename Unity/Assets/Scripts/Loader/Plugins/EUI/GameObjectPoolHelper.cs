﻿using System;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace ET.Client
{
    public static class GameObjectPoolHelper
    {
        private static Dictionary<string, GameObjectPool> poolDict = new Dictionary<string, GameObjectPool>();
        
        public static void InitPool( string poolName, int size, PoolInflationType type = PoolInflationType.DOUBLE)
        {
            if (poolDict.ContainsKey(poolName))
            {
                return;
            }
            else
            {
                try
                {
                    GameObject pb = GetGameObjectByResType(poolName);
                    if (pb == null)
                    {
                        Debug.LogError("[ResourceManager] Invalide prefab name for pooling :" + poolName);
                        return;
                    }

                    
                    poolDict[poolName] = new GameObjectPool(poolName, pb, GameObject.Find("Global/PoolRoot"), size, type);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        
        public static async ETTask<GameObject> GetGameObjectAsync(string gameObjectPath)
        {
            GameObject pb = null;
            pb = await ResourcesComponent.Instance.LoadAssetAsync<GameObject>(gameObjectPath);
            return pb;
        }
        
        public static async ETTask InitPoolWithPathAsync( string poolName, string assetPath,int size, PoolInflationType type = PoolInflationType.DOUBLE)
        {
            if (poolDict.ContainsKey(poolName))
            {
                return;
            }
            else
            {
                try
                {
                    GameObject pb = await GetGameObjectAsync(assetPath);
                    if (pb == null)
                    {
                        Debug.LogError("[ResourceManager] Invalide prefab name for pooling :" + poolName);
                        return;
                    }
                    
                    poolDict[poolName] = new GameObjectPool(poolName, pb, GameObject.Find("Global/PoolRoot"), size, type);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        
        
        /// <summary>
        /// Returns an available object from the pool 
        /// OR null in case the pool does not have any object available & can grow size is false.
        /// </summary>
        /// <OtherParam name="poolName"></OtherParam>
        /// <returns></returns>
        public static GameObject GetObjectFromPool( string poolName,    bool autoActive = true, int autoCreate = 0)
        {
            GameObject result = null;

            if (!poolDict.ContainsKey(poolName) && autoCreate > 0)
            {
                InitPool(poolName, autoCreate, PoolInflationType.INCREMENT);
            }

            if (poolDict.ContainsKey(poolName))
            {
                GameObjectPool pool = poolDict[poolName];
                result = pool.NextAvailableObject(autoActive);
                //scenario when no available object is found in pool
#if UNITY_EDITOR
                if (result == null)
                {
                    Debug.LogWarning("[ResourceManager]:No object available in " + poolName);
                }
#endif
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError("[ResourceManager]:Invalid pool name specified: " + poolName);
            }
#endif
            return result;
        }

        
        /// <summary>
        /// Returns an available object from the pool 
        /// OR null in case the pool does not have any object available & can grow size is false.
        /// </summary>
        /// <OtherParam name="poolName"></OtherParam>
        /// <returns></returns>
        public static async ETTask<GameObject> GetObjectFromPoolAsync( string poolName,  string assetPath,  bool autoActive = true, int autoCreate = 0)
        {
            GameObject result = null;

            if (!poolDict.ContainsKey(poolName) && autoCreate > 0)
            {
                await InitPoolWithPathAsync(poolName, assetPath,autoCreate, PoolInflationType.INCREMENT);
            }

            if (poolDict.ContainsKey(poolName))
            {
                GameObjectPool pool = poolDict[poolName];
                result = pool.NextAvailableObject(autoActive);
                //scenario when no available object is found in pool
#if UNITY_EDITOR
                if (result == null)
                {
                    Debug.LogWarning("[ResourceManager]:No object available in " + poolName);
                }
#endif
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError("[ResourceManager]:Invalid pool name specified: " + poolName);
            }
#endif
            return result;
        }

        
        /// <summary>
        /// Return obj to the pool
        /// </summary>
        /// <OtherParam name="go"></OtherParam>
        public static void ReturnObjectToPool( GameObject go)
        {
            PoolObject po = go.GetComponent<PoolObject>();
            if (po == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Specified object is not a pooled instance: " + go.name);
#endif
            }
            else
            {
                GameObjectPool pool = null;
                if (poolDict.TryGetValue(po.poolName, out pool))
                {
                    pool.ReturnObjectToPool(po);
                }
#if UNITY_EDITOR
                else
                {
                    Debug.LogWarning("No pool available with name: " + po.poolName);
                }
#endif
            }
        }

        /// <summary>
        /// Return obj to the pool
        /// </summary>
        /// <OtherParam name="t"></OtherParam>
        public static void ReturnTransformToPool( Transform t)
        {
            if (t == null)
            {
#if UNITY_EDITOR
                Debug.LogError("[ResourceManager] try to return a null transform to pool!");
#endif
                return;
            }
            ReturnObjectToPool(t.gameObject);
        }

        public static GameObject GetGameObjectByResType( string poolName)
        {
            // GameObject pb = ResourcesComponent.Instance.LoadAssetSync<GameObject>( $"Assets/Bundles/UI/Item/{poolName}.prefab");
            GameObject pb = ResourcesComponent.Instance.LoadAssetSync<GameObject>( poolName);
            return pb;
        }
    }
}