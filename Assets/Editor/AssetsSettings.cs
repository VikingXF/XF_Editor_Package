//=======================================================
// 作者：xf
// 描述：模型/贴图导入批处理
//=======================================================

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace BabybusAssetsSettings
{

    public class AssetsSettings : EditorWindow
    {
        int MainToolbar = 0;
        string[] MainToolbarStrings = new string[] { "游戏资源批处理", "动画资源批处理" };

        //动画批处理变量
        #region
        int AnimationtoolbarInt = 0;
        string[] AnimationtoolbarStrings = new string[] { "贴图", "模型", "材质" };

        
        #endregion

        //游戏批处理变量
        #region
        int toolbarInt = 0;
        string[] toolbarStrings = new string[] { "贴图/UI", "模型", "材质批处理", "贴图特殊压缩" };

        //static ImageCompression imagecompression = new ImageCompression();


        //贴图变量
        #region
        static bool IsPowerof2 = true;
        static bool IsMipMaps = false;
        static bool RWenabled = false;
        static bool IsCompression = true;

        // 2的N次方设置枚举
        public enum NpotScaleNum
        {
            None,
            ToNearest,
            ToLarger,
            ToSmaller,
        }
        static private NpotScaleNum npotScaleNum = NpotScaleNum.ToNearest;

        // 贴图类型设置
        public enum TextureType
        {
            Default,
            Sprite,      
        }
        static private TextureType textureType = TextureType.Default;
        #endregion

        //模型变量
        #region

        static bool MeshCom = true;
        static bool MeshRW = false;  
        static bool MeshOpt = true;
        static bool MeshGC = false;
        static bool MeshKQ = false;
        static bool MeshIA = true;
        static bool MeshAnimCompression = true;
        static bool MeshImportMaterials = true;
        #endregion

        //材质变量
        #region
        public enum MaterilaType
        {
            Unlit,
            Mobile,
        }
        static private MaterilaType MaterilasType = MaterilaType.Unlit;
        #endregion

        #endregion

        [MenuItem("Tools/资源设置批处理工具")]
        public static void ShowWindow()
        {

            EditorWindow editorWindow = EditorWindow.GetWindow(typeof(AssetsSettings));
            editorWindow.autoRepaintOnSceneChange = true;
            editorWindow.Show();
            editorWindow.titleContent.text = "资源批处理2.0";
        }

        void OnEnable()
        {
    
            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
            SceneView.onSceneGUIDelegate += this.OnSceneGUI;
        }
        void OnDestroy()
        {
            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;

        }

        void OnSceneGUI(SceneView sceneView)
        {



        }

        private void OnGUI()
        {
            GUILayout.Space(10f);
            MainToolbar = GUILayout.Toolbar(MainToolbar, MainToolbarStrings, GUILayout.Height(28));
            

            switch (MainToolbar)
            {
                case 0:
                    GameResourceBatchingUI();
                    
                    break;
                case 1:
                    AnimationResourceBatchingUI();
                    break;
            }

            

        }

        //动画资源
        #region
        //动画资源批处理UI
        #region
        private void AnimationResourceBatchingUI()
        {
            AnimationtoolbarInt = GUILayout.Toolbar(AnimationtoolbarInt, AnimationtoolbarStrings);
            switch (AnimationtoolbarInt)
            {
                case 0:
                    AnimationMapProcessingUI();
                    break;
                case 1:
                    AnimationModelProcessingUI();
                    break;
                case 2:
                    AnimationMaterilasProcessingUI();
                    break;

            }

        }
        #endregion

        //动画贴图处理UI
        #region
        private void AnimationMapProcessingUI()
        {
            GUILayout.BeginVertical("box", GUILayout.Width(200));
            GUILayout.Label("贴图处理");
            GUILayout.BeginHorizontal();
        
            if (GUILayout.Button("动画贴图批处理", GUILayout.Width(240), GUILayout.Height(30)))
            {

                SelectedAnimationTexChange();

            }
            GUILayout.EndVertical();
        }
        #endregion
        
        
        
        //动画模型处理UI
        #region
        private void AnimationModelProcessingUI()
        {
            GUILayout.BeginVertical("box", GUILayout.Width(200));
            GUILayout.Label("动画模型设置");
            if (GUILayout.Button("动画模型批处理", GUILayout.Width(240), GUILayout.Height(30)))
            {

                SelectedAnimationMeshChange();

            }

            GUILayout.EndVertical();
        }
        #endregion

        //动画材质处理UI
        #region
        private void AnimationMaterilasProcessingUI()
        {
            GUILayout.BeginVertical("box", GUILayout.Width(200));
            GUILayout.Label("材质处理");
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("动画材质批处理", GUILayout.Width(240), GUILayout.Height(30)))
            {

                SelectedAnimationMaterilasChange();

            }
            GUILayout.EndVertical();
        }
        #endregion

        //动画贴图处理
        #region

        static void SelectedAnimationTexChange()
        {

            Object[] textures = GetSelectedTextures();

            if (textures.Length == 0)
            {
                EditorUtility.DisplayDialog("警告", "选择一个包含贴图的文件夹或者单独贴图！", "OK");
                return;
            }

            foreach (Texture2D texture in textures)
            {
                string path = AssetDatabase.GetAssetPath(texture);
                TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

                //MipMaop设置
                textureImporter.mipmapEnabled = false;

                //贴图可读写设置
                textureImporter.isReadable = false;

                //贴图压缩设置
                if (!textureImporter.DoesSourceTextureHaveAlpha())//不带alpha通道的
                {
                    textureImporter.alphaIsTransparency = false;
                    textureImporter.SetPlatformTextureSettings("PC", 4096, TextureImporterFormat.RGB24);

                }
                else  //带alpha通道的
                {
                    textureImporter.alphaIsTransparency = true;
                    textureImporter.SetPlatformTextureSettings("PC", 4096, TextureImporterFormat.RGBA32);

                }


                AssetDatabase.ImportAsset(path);
            }
        }

        #endregion
        
        //动画模型处理
        #region
        static void SelectedAnimationMeshChange()
        {
            Object[] meshs = GetSelectedModels();

            if (meshs.Length == 0)
            {
                EditorUtility.DisplayDialog("警告", "选择一个包含模型的文件夹或者单独模型！", "OK");
                return;
            }

            for (int i = 0; i < meshs.Length; i++)
            {
                var a = meshs[i] as GameObject;
                if (a != null)
                {
                    string path = AssetDatabase.GetAssetPath(a);
                    string FBXpath = path.Substring(path.Length - 4, 4);
                    if (FBXpath == ".FBX")
                    {
                        ModelImporter modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;

                        //Animation
                        modelImporter.importAnimation = true;

                        modelImporter.animationCompression = ModelImporterAnimationCompression.Off;



                        //Materials
                        modelImporter.useSRGBMaterialColor = true;
                        modelImporter.materialLocation = ModelImporterMaterialLocation.External;
                        modelImporter.materialName = ModelImporterMaterialName.BasedOnTextureName;
                        modelImporter.materialSearch = ModelImporterMaterialSearch.RecursiveUp;



                        AssetDatabase.ImportAsset(path);
                    }
                    
                }

            }


        }

        #endregion

        
        //动画材质球处理
        #region
        static void SelectedAnimationMaterilasChange()
        {
            Object[] Materilas = GetSelectedMaterilas();
            if (Materilas.Length == 0)
            {
                EditorUtility.DisplayDialog("警告", "选择一个包含材质球的文件夹或者单独材质球！", "OK");
                return;
            }
            foreach (Material material in Materilas)
            {
                if (material.shader == Shader.Find("HDRP/Lit"))
                {
                    material.SetColor("_BaseColor", Color.white);
                    material.SetFloat("_Metallic", 0f);
                    material.SetFloat("_Smoothness", 0f);

                    if (material.GetTexture("_BaseColorMap") != null)
                    {

                        string path = AssetDatabase.GetAssetPath(material.GetTexture("_BaseColorMap"));
                        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

                        if (textureImporter.DoesSourceTextureHaveAlpha())//带alpha通道的
                        {
                            material.SetFloat("_SurfaceType", 1f);
                        }
                        else  //不带alpha通道的
                        {
                            material.SetFloat("_SurfaceType", 0f);

                        }
                    }

                }
        
            }

        }


        #endregion
        
        
        #endregion

        //游戏资源
        #region

        //游戏资源批处理UI
        #region

        private void GameResourceBatchingUI()
        {
            toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings);
            switch (toolbarInt)
            {
                case 0:

                    MapProcessingUI();
                    break;
                case 1:
                    ModelProcessingUI();
                    break;
                case 2:
                    MaterialProcessingUI();
                    break;
                case 3:
                    TextureSpecialProcessingUI();
                    break;
            }
        }

        #endregion
        
        //贴图处理UI
        #region
        private void MapProcessingUI()
        {
            GUILayout.BeginVertical("box", GUILayout.Width(200));
            GUILayout.Label("贴图设置");

            GUILayout.BeginHorizontal();
            textureType = (TextureType)EditorGUILayout.EnumPopup(new GUIContent("Texture Type"), textureType);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            IsPowerof2 = GUILayout.Toggle(IsPowerof2, "");
            npotScaleNum = (NpotScaleNum)EditorGUILayout.EnumPopup(new GUIContent("Non Power of 2"), npotScaleNum);
            GUILayout.EndHorizontal();

            IsMipMaps = GUILayout.Toggle(IsMipMaps, "    Generate Mip Maps");

            RWenabled = GUILayout.Toggle(RWenabled, "    Read/Write Enabled");

            IsCompression = GUILayout.Toggle(IsCompression, "    Compression(Max Size:1024)");


            if (GUILayout.Button("贴图批处理", GUILayout.Width(240), GUILayout.Height(30)))
            {

                SelectedTexChange();

            }
            GUILayout.EndVertical();
        }
        #endregion

        //模型处理UI
        #region
        private void ModelProcessingUI()
        {
            GUILayout.BeginVertical("box", GUILayout.Width(200));
            GUILayout.Label("模型设置");

            MeshCom = GUILayout.Toggle(MeshCom, "    Mesh Compression");
            MeshRW = GUILayout.Toggle(MeshRW, "    Read/Write Enabled");
            MeshOpt = GUILayout.Toggle(MeshOpt, "    Optimze Mesh");
            MeshGC = GUILayout.Toggle(MeshGC, "    Generate Colliders");
            MeshKQ = GUILayout.Toggle(MeshKQ, "    Keep Quads");
            MeshIA = GUILayout.Toggle(MeshIA, "    Import Animation");
            MeshAnimCompression = GUILayout.Toggle(MeshAnimCompression, "    Anim Compression()");
            MeshImportMaterials = GUILayout.Toggle(MeshImportMaterials, "    Import Materials");

            if (GUILayout.Button("模型批处理", GUILayout.Width(240), GUILayout.Height(30)))
            {

                SelectedMeshChange();

            }

            GUILayout.EndVertical();
        }
        #endregion

        //材质球处理UI
        #region
        private void MaterialProcessingUI()
        {
            GUILayout.BeginVertical("box", GUILayout.Width(200));
            GUILayout.Label("材质设置");
            GUILayout.BeginHorizontal();
            MaterilasType = (MaterilaType)EditorGUILayout.EnumPopup(new GUIContent("材质选项是否受光"), MaterilasType);

            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("box", GUILayout.Width(200));
            GUILayout.Label("说明：Unlit不接受光，mobile接受光           ");
            GUILayout.EndVertical();

            if (GUILayout.Button("材质处理", GUILayout.Width(240), GUILayout.Height(30)))
            {

                SelectedMaterilasChange();

            }

            GUILayout.EndVertical();
        }
        #endregion

        //特殊贴图处理UI
        #region
        private void TextureSpecialProcessingUI()
        {
            GUILayout.BeginVertical("box", GUILayout.Width(300));
            GUILayout.Label("贴图特殊处理");

            if (GUILayout.Button("带Alpha RGBA16+Dither处理不带Alpha RGB565处理", GUILayout.Width(300), GUILayout.Height(30)))
            {
                //SpecialTexChange();

            }

            GUILayout.EndVertical();
        }
        #endregion


        //游戏贴图处理
        #region
        static void SelectedTexChange()
        {

            Object[] textures = GetSelectedTextures();
        
            if (textures.Length == 0)
            {
                EditorUtility.DisplayDialog("警告", "选择一个包含贴图的文件夹或者单独贴图！", "OK");
                return;
            }
    
            foreach (Texture2D texture in textures)
            {
                string path = AssetDatabase.GetAssetPath(texture);
                TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

                //贴图类型设置
                switch (textureType)
                {
                    case TextureType.Default:
                        textureImporter.textureType = TextureImporterType.Default;
                        //2的N次方设置
                        if (IsPowerof2 == true)
                        {
                            switch (npotScaleNum)
                            {
                                case NpotScaleNum.None:
                                    textureImporter.npotScale = TextureImporterNPOTScale.None;
                                    break;
                                case NpotScaleNum.ToNearest:
                                    textureImporter.npotScale = TextureImporterNPOTScale.ToNearest;
                                    break;
                                case NpotScaleNum.ToLarger:
                                    textureImporter.npotScale = TextureImporterNPOTScale.ToLarger;
                                    break;
                                case NpotScaleNum.ToSmaller:
                                    textureImporter.npotScale = TextureImporterNPOTScale.ToSmaller;
                                    break;
                            }
                        }
                        break;
                    case TextureType.Sprite:
                        textureImporter.textureType = TextureImporterType.Sprite;
                        textureImporter.alphaIsTransparency = true;
                        break;
                }
            
                //MipMaop设置
                textureImporter.mipmapEnabled = IsMipMaps;

                //贴图可读写设置
                textureImporter.isReadable = RWenabled;

                //贴图压缩设置
                if (IsCompression == true )                                                                                                                                                                                                     
                {

                    if (!textureImporter.DoesSourceTextureHaveAlpha())//不带alpha通道的
                    {
                        textureImporter.alphaIsTransparency = false;

                        //TextureImporterPlatformSettings AndroidNoAlphatextureSettings = new TextureImporterPlatformSettings();
                        //AndroidNoAlphatextureSettings.name = "Android";
                        //AndroidNoAlphatextureSettings.maxTextureSize = 1024;
                        //AndroidNoAlphatextureSettings.format = TextureImporterFormat.ETC_RGB4;

                        //TextureImporterPlatformSettings iPhoneNoAlphatextureSettings = new TextureImporterPlatformSettings();
                        //iPhoneNoAlphatextureSettings.name = "iPhone";
                        //iPhoneNoAlphatextureSettings.maxTextureSize = 1024;
                        //iPhoneNoAlphatextureSettings.format = TextureImporterFormat.PVRTC_RGB4;

                        //textureImporter.SetPlatformTextureSettings(AndroidNoAlphatextureSettings);
                        //textureImporter.SetPlatformTextureSettings(iPhoneNoAlphatextureSettings);
                        textureImporter.SetPlatformTextureSettings("Android", 1024, TextureImporterFormat.ETC_RGB4);
                        textureImporter.SetPlatformTextureSettings("iPhone", 1024, TextureImporterFormat.PVRTC_RGB4);
                        //textureImporter.SetPlatformTextureSettings("iPhone", 1024, TextureImporterFormat.ASTC_RGBA_6x6);
                    }
                    else  //带alpha通道的
                    {
                        textureImporter.alphaIsTransparency = true;

                        //TextureImporterPlatformSettings AndroidAlphatextureSettings = new TextureImporterPlatformSettings();
                        //AndroidAlphatextureSettings.name = "Android";
                        //AndroidAlphatextureSettings.maxTextureSize = 1024;
                        //AndroidAlphatextureSettings.format = TextureImporterFormat.ETC2_RGBA8;

                        //TextureImporterPlatformSettings iPhoneAlphatextureSettings = new TextureImporterPlatformSettings();
                        //iPhoneAlphatextureSettings.name = "iPhone";
                        //iPhoneAlphatextureSettings.maxTextureSize = 1024;
                        //iPhoneAlphatextureSettings.format = TextureImporterFormat.PVRTC_RGBA4;

                        //textureImporter.SetPlatformTextureSettings(AndroidAlphatextureSettings);
                        //textureImporter.SetPlatformTextureSettings(iPhoneAlphatextureSettings);

                        textureImporter.SetPlatformTextureSettings("Android", 1024, TextureImporterFormat.ETC2_RGBA8);
                       textureImporter.SetPlatformTextureSettings("iPhone", 1024, TextureImporterFormat.PVRTC_RGBA4);
                       //textureImporter.SetPlatformTextureSettings("iPhone", 1024, TextureImporterFormat.ASTC_RGBA_6x6);
                    }


                }

            

                AssetDatabase.ImportAsset(path);
            }
        }
        static Object[] GetSelectedTextures()
        {
            return Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
        }
        #endregion


        //游戏模型处理
        #region
        static void SelectedMeshChange()
        {
            Object[] meshs = GetSelectedModels();

            if (meshs.Length == 0)
            {
                EditorUtility.DisplayDialog("警告", "选择一个包含模型的文件夹或者单独模型！", "OK");
                return;
            }
        

            for (int i = 0; i < meshs.Length; i++)
            {
                var a = meshs[i] as GameObject;
                if (a != null)
                {
                    string path = AssetDatabase.GetAssetPath(a);
                    ModelImporter modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;

                    //Model
                    if (MeshCom == true)
                    {
                        modelImporter.meshCompression = ModelImporterMeshCompression.Medium;
                    }
                
                    modelImporter.isReadable = MeshRW;
                    if (MeshOpt)
                    {
                        modelImporter.optimizeMeshVertices = true;
                        modelImporter.optimizeMeshPolygons = true;

                    }
                    
                    //modelImporter.optimizeMesh = MeshOpt;
                    modelImporter.addCollider = MeshGC;
                    modelImporter.keepQuads = MeshKQ;

                    //Animation
                    Debug.Log("MeshImportMaterials" + MeshImportMaterials);
                    modelImporter.importAnimation = MeshIA;
                    if (MeshAnimCompression == true)
                    {
                        modelImporter.animationCompression = ModelImporterAnimationCompression.Optimal;
                        modelImporter.animationRotationError = 1;
                        modelImporter.animationPositionError = 1;
                        modelImporter.animationScaleError = 1;
                    }

                    //Materials
                    if (MeshImportMaterials)
                    {
                        modelImporter.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
                    }
                    else
                    {
                        modelImporter.materialImportMode = ModelImporterMaterialImportMode.None;

                    }
                    //modelImporter.importMaterials = MeshImportMaterials;
                    if (MeshImportMaterials == true)
                    {
                        modelImporter.materialLocation = ModelImporterMaterialLocation.External;
                        modelImporter.materialName = ModelImporterMaterialName.BasedOnTextureName;
                        modelImporter.materialSearch = ModelImporterMaterialSearch.RecursiveUp;
                    }
                    


                    AssetDatabase.ImportAsset(path);
                }

            }

            
        }
        static Object[] GetSelectedModels()
        {

            return Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        }

        #endregion


        //游戏材质球处理
        #region
        static void SelectedMaterilasChange()
        {
            Object[] Materilas = GetSelectedMaterilas();
            if (Materilas.Length == 0)
            {
                EditorUtility.DisplayDialog("警告", "选择一个包含材质球的文件夹或者单独材质球！", "OK");
                return;
            }
            foreach (Material material in Materilas)
            {
                if (material.mainTexture !=null)
                {

                    string path2 = AssetDatabase.GetAssetPath(material.mainTexture);
                    TextureImporter textureImporter2 = AssetImporter.GetAtPath(path2) as TextureImporter;

                    if (!textureImporter2.DoesSourceTextureHaveAlpha())//不带alpha通道的
                    {
                        switch (MaterilasType)
                        {
                            case MaterilaType.Unlit:
                                material.shader = Shader.Find("Babybus/Unlit/Texture");
                                break;
                            case MaterilaType.Mobile:
                                material.shader = Shader.Find("Babybus/Mobile/Diffuse");
                                break;
                        }
                    }

                    else  //带alpha通道的
                    {
                        switch (MaterilasType)
                        {
                            case MaterilaType.Unlit:
                                material.shader = Shader.Find("Babybus/Unlit/Transparent");
                                break;
                            case MaterilaType.Mobile:
                                material.shader = Shader.Find("Babybus/Mobile/Transparent-VertexLit");
                                material.color = Color.white;
                                break;
                        }

                    }
                }
                else
                {
                    Debug.Log(material.name);
                }
                
                

            }


        }

        static Object[] GetSelectedMaterilas()
        {

            return Selection.GetFiltered(typeof(Material), SelectionMode.DeepAssets);
        }

        #endregion
        
        //特殊贴图处理
        #region
        static void SpecialTexChange()
        {
            
            Object[] textures = GetSelectedTextures();
        
            if (textures.Length == 0)
            {
                EditorUtility.DisplayDialog("警告", "选择一个包含贴图的文件夹或者单独贴图！", "OK");
                return;
            }
    
            foreach (Texture2D texture in textures)
            {
                string path = AssetDatabase.GetAssetPath(texture);
                TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

                 if (!textureImporter.DoesSourceTextureHaveAlpha())//不带alpha通道的
                {
                   
                    //textureImporter.isReadable = true;
                    AssetDatabase.ImportAsset(path);
                    //imagecompression.TextureRGB565(texture);
                    //textureImporter.isReadable = false;

                }
                else  //带alpha通道的
                {
                   
                   // textureImporter.isReadable = true;
                    AssetDatabase.ImportAsset(path);
                    //imagecompression.TextureRGBA4444(texture);
                    //imagecompression.TextureRGB565(texture);
                    //textureImporter.isReadable = false;

                }


                //AssetDatabase.ImportAsset(path);
            }
        }
        
        #endregion

        #endregion

    }

    #endif
}