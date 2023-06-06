using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceCollection : MonoBehaviour
{
    
    public ResourceCollection() { }


    Dictionary<String, Resource> resources = new Dictionary<String, Resource>() { };

    public double getNothing()
    {
        return 0.0;
    }

    public void addResource(String resourceName, Resource resource)
    {
        resources.Add(resourceName, resource);
    }

    public Dictionary<String, Resource> getResourceCollection()
    {
        return resources;
    }

    public Resource getResource(String resourceName)
    {
        //use TryGetValue() to get a value of unknown key ( resource name )
        Resource outResource;

        if (resources.TryGetValue(resourceName, out outResource))
        {
            return outResource;
        }

        return new Resource();
    }


    
}
