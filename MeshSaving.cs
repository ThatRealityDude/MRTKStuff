using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Management;
using Unity.XR.CoreUtils;
using LegacyMeshId = UnityEngine.XR.MeshId;
using UnityEngine.XR.ARFoundation;
using System;
using UnityEditor;
using System.IO;
using Unity.VisualScripting;

public class MeshSaving: MonoBehaviour
{
    [SerializeField] private ARMeshManager meshManager;
    private Mesh newMesh;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Mesh prefabMesh;
    // Start is called before the first frame update
    void Start()
    {
        newMesh = new Mesh();
        newMesh.name = "EmptyMesh";
        GetComponent<MeshFilter>().mesh = newMesh;
        newMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
    }
    
    public void getMesh()
    {
        meshManager.transform.localScale = Vector3.one * 100f;
        MeshFilter[] meshFilters = new MeshFilter[meshManager.meshes.Count];
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < meshManager.meshes.Count; i++)
        {
            meshFilters[i] = meshManager.meshes[i];
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }
        newMesh.CombineMeshes(combine); //Rainbow
    }
    public void saveMesh()
    {
        if (!Directory.Exists("Assets/test"))
        {
            AssetDatabase.CreateFolder("Assets", "test");
        }

        string localPath = "Assets/test/" + gameObject.name + ".prefab";

        // Make sure the file name is unique, in case an existing Prefab has the same name.
        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
        AssetDatabase.CreateAsset(newMesh, localPath);
    }

    public void compareMesh()
    {
        prefabMesh.GameObject().transform.position = newMesh.GameObject().transform.position;
        prefabMesh.GameObject().transform.rotation = newMesh.GameObject().transform.rotation;
    }

    public Mesh getCurrentMesh()
    {
        Mesh filler = new Mesh();
        MeshFilter[] meshFilters = new MeshFilter[meshManager.meshes.Count];
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < meshManager.meshes.Count; i++)
        {
            meshFilters[i] = meshManager.meshes[i];
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }
        filler.CombineMeshes(combine);
        return filler;
    }
}
