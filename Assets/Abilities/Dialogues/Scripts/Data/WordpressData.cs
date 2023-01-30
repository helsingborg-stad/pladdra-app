using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pladdra.Data
{
    /// <summary>
    /// Helper object for deserializing json from wordpress.
    /// </summary>
    [System.Serializable]
    public class WordpressData
    {
        public string id;
        public string name;
        public Title title;
    }

    [System.Serializable]
    public class Title
    {
        public string rendered;
    }

    [System.Serializable]
    public class Acf
    {

    }

}