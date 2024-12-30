Shader "YAPU/SpindaShaderCompiled"
{
    Properties
    {
        [NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
        _SpriteSheet("SpriteSheet", 2D) = "white" {}
        _Flipbook("Flipbook", Vector) = (52, 1, 0, 0)
        _Speed("Speed", Float) = 10
        [ToggleUI]_SpecificIndex("SpecificIndex", Float) = 1
        _MinIndex("MinIndex", Float) = 0
        _MaxIndex("MaxIndex", Float) = 51
        [ToggleUI]_IsShadow("IsShadow", Float) = 0
        _ShadowColor("ShadowColor", Color) = (0.2264151, 0.2264151, 0.2264151, 0)
        _ShadowAlpha("ShadowAlpha", Range(0, 1)) = 0.6
        [ToggleUI]_AutoAnimate("AutoAnimate", Float) = 1
        _SpriteIndex("SpriteIndex", Int) = 0
        [NoScaleOffset]_Spot1("Spot1", 2D) = "white" {}
        [NoScaleOffset]_Spot2("Spot2", 2D) = "white" {}
        [NoScaleOffset]_Spot3("Spot3", 2D) = "white" {}
        [NoScaleOffset]_Spot4("Spot4", 2D) = "white" {}
        _Spot1Coords("Spot1Coords", Vector) = (8, 8, 0, 0)
        _Spot2Coords("Spot2Coords", Vector) = (8, 8, 0, 0)
        _Spot3Coords("Spot3Coords", Vector) = (8, 8, 0, 0)
        _Spot4Coords("Spot4Coords", Vector) = (8, 8, 0, 0)
        [NoScaleOffset]_SpotPositions("SpotPositions", 2D) = "white" {}
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
            // DisableBatching: <None>
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalSpriteUnlitSubTarget"
            "PreviewType"="Plane"
        }
        Pass
        {
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "Universal2D"
            }
        
        // Render State
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEUNLIT
        #define ALPHA_CLIP_THRESHOLD 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 color : INTERP1;
             float3 positionWS : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.color.xyzw = input.color;
            output.positionWS.xyz = input.positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.color = input.color.xyzw;
            output.positionWS = input.positionWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _SpriteSheet_TexelSize;
        float4 _SpriteSheet_ST;
        float2 _Flipbook;
        float _Speed;
        float4 _MainTex_TexelSize;
        float4 _ShadowColor;
        float _IsShadow;
        float _ShadowAlpha;
        float _AutoAnimate;
        float _SpriteIndex;
        float _MaxIndex;
        float _SpecificIndex;
        float _MinIndex;
        float2 _Spot1Coords;
        float2 _Spot2Coords;
        float2 _Spot3Coords;
        float2 _Spot4Coords;
        float4 _Spot1_TexelSize;
        float4 _Spot2_TexelSize;
        float4 _Spot3_TexelSize;
        float4 _Spot4_TexelSize;
        float4 _SpotPositions_TexelSize;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SpriteSheet);
        SAMPLER(sampler_SpriteSheet);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_Spot1);
        SAMPLER(sampler_Spot1);
        TEXTURE2D(_Spot2);
        SAMPLER(sampler_Spot2);
        TEXTURE2D(_Spot3);
        SAMPLER(sampler_Spot3);
        TEXTURE2D(_Spot4);
        SAMPLER(sampler_Spot4);
        TEXTURE2D(_SpotPositions);
        SAMPLER(sampler_SpotPositions);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Floor_float(float In, out float Out)
        {
            Out = floor(In);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Modulo_float(float A, float B, out float Out)
        {
            Out = fmod(A, B);
        }
        
        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_Flipbook_InvertY_float (float2 UV, float Width, float Height, float Tile, float2 Invert, out float2 Out)
        {
            Tile = floor(fmod(Tile + float(0.00001), Width*Height));
            float2 tileCount = float2(1.0, 1.0) / float2(Width, Height);
            float base = floor((Tile + float(0.5)) * tileCount.x);
            float tileX = (Tile - Width * base);
            float tileY = (Invert.y * Height - (base + Invert.y * 1));
            Out = (UV + float2(tileX, tileY)) * tileCount;
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Maximum_float(float A, float B, out float Out)
        {
            Out = max(A, B);
        }
        
        void Unity_Blend_Multiply_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
        {
            Out = Base * Blend;
            Out = lerp(Base, Out, Opacity);
        }
        
        void Unity_Preview_float4(float4 In, out float4 Out)
        {
            Out = In;
        }
        
        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_Preview_float(float In, out float Out)
        {
            Out = In;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_d33558c987894df9bf624c76589f84ee_Out_0_Boolean = _IsShadow;
            float4 _Property_d0bf811957fb4be4acc48ac3524321e9_Out_0_Vector4 = _ShadowColor;
            UnityTexture2D _Property_97fd4aa82e05435894a5923b475c61fd_Out_0_Texture2D = UnityBuildTexture2DStruct(_SpriteSheet);
            float2 _Property_a9897e89a2234f828d69cd756b02f8ec_Out_0_Vector2 = _Flipbook;
            float _Split_3d7acd9d31ed4dc791759fa0f2ba28e2_R_1_Float = _Property_a9897e89a2234f828d69cd756b02f8ec_Out_0_Vector2[0];
            float _Split_3d7acd9d31ed4dc791759fa0f2ba28e2_G_2_Float = _Property_a9897e89a2234f828d69cd756b02f8ec_Out_0_Vector2[1];
            float _Split_3d7acd9d31ed4dc791759fa0f2ba28e2_B_3_Float = 0;
            float _Split_3d7acd9d31ed4dc791759fa0f2ba28e2_A_4_Float = 0;
            float _Property_df4ef9ac52f54ff891606e4915a58b47_Out_0_Boolean = _AutoAnimate;
            float _Property_ceb17021a85a40469f6d4da63e231c35_Out_0_Boolean = _SpecificIndex;
            float _Property_42cb7d43d6144dd38ec1c722c9bda400_Out_0_Float = _Speed;
            float _Multiply_0dcaf02843384467875717b4b0671a50_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_42cb7d43d6144dd38ec1c722c9bda400_Out_0_Float, _Multiply_0dcaf02843384467875717b4b0671a50_Out_2_Float);
            float _Floor_8be86eeacaf94b0db24577bcbfcdbdfd_Out_1_Float;
            Unity_Floor_float(_Multiply_0dcaf02843384467875717b4b0671a50_Out_2_Float, _Floor_8be86eeacaf94b0db24577bcbfcdbdfd_Out_1_Float);
            float _Property_9e5c02c283504d1caca6e793828cb16b_Out_0_Float = _MinIndex;
            float _Add_751390100f9e49ebaa806f60f32d70c0_Out_2_Float;
            Unity_Add_float(_Floor_8be86eeacaf94b0db24577bcbfcdbdfd_Out_1_Float, _Property_9e5c02c283504d1caca6e793828cb16b_Out_0_Float, _Add_751390100f9e49ebaa806f60f32d70c0_Out_2_Float);
            float _Property_855ccdcd2d34412aba0ac01bb659ec4d_Out_0_Float = _MaxIndex;
            float _Subtract_e917f3dbe48740ce818c535ba035c918_Out_2_Float;
            Unity_Subtract_float(_Property_855ccdcd2d34412aba0ac01bb659ec4d_Out_0_Float, _Property_9e5c02c283504d1caca6e793828cb16b_Out_0_Float, _Subtract_e917f3dbe48740ce818c535ba035c918_Out_2_Float);
            float _Modulo_7b3bd6bae30147b688a716ebf2bc8092_Out_2_Float;
            Unity_Modulo_float(_Add_751390100f9e49ebaa806f60f32d70c0_Out_2_Float, _Subtract_e917f3dbe48740ce818c535ba035c918_Out_2_Float, _Modulo_7b3bd6bae30147b688a716ebf2bc8092_Out_2_Float);
            float _Branch_d447bbc59ef6419a8bf995775d5e6e72_Out_3_Float;
            Unity_Branch_float(_Property_ceb17021a85a40469f6d4da63e231c35_Out_0_Boolean, _Modulo_7b3bd6bae30147b688a716ebf2bc8092_Out_2_Float, _Floor_8be86eeacaf94b0db24577bcbfcdbdfd_Out_1_Float, _Branch_d447bbc59ef6419a8bf995775d5e6e72_Out_3_Float);
            float _Property_89da1bdd17c64d2cad478880089d481b_Out_0_Float = _SpriteIndex;
            float _Branch_6e389d225a3d4b4382f659549d8957bb_Out_3_Float;
            Unity_Branch_float(_Property_df4ef9ac52f54ff891606e4915a58b47_Out_0_Boolean, _Branch_d447bbc59ef6419a8bf995775d5e6e72_Out_3_Float, _Property_89da1bdd17c64d2cad478880089d481b_Out_0_Float, _Branch_6e389d225a3d4b4382f659549d8957bb_Out_3_Float);
            float2 _Flipbook_cf3c7b4cd4834ca7baf944c89ae7e107_Out_4_Vector2;
            float2 _Flipbook_cf3c7b4cd4834ca7baf944c89ae7e107_Invert = float2 (0, 1);
            Unity_Flipbook_InvertY_float(IN.uv0.xy, _Split_3d7acd9d31ed4dc791759fa0f2ba28e2_R_1_Float, _Split_3d7acd9d31ed4dc791759fa0f2ba28e2_G_2_Float, _Branch_6e389d225a3d4b4382f659549d8957bb_Out_3_Float, _Flipbook_cf3c7b4cd4834ca7baf944c89ae7e107_Invert, _Flipbook_cf3c7b4cd4834ca7baf944c89ae7e107_Out_4_Vector2);
            float4 _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_97fd4aa82e05435894a5923b475c61fd_Out_0_Texture2D.tex, _Property_97fd4aa82e05435894a5923b475c61fd_Out_0_Texture2D.samplerstate, _Property_97fd4aa82e05435894a5923b475c61fd_Out_0_Texture2D.GetTransformedUV(_Flipbook_cf3c7b4cd4834ca7baf944c89ae7e107_Out_4_Vector2) );
            float _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_R_4_Float = _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_RGBA_0_Vector4.r;
            float _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_G_5_Float = _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_RGBA_0_Vector4.g;
            float _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_B_6_Float = _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_RGBA_0_Vector4.b;
            float _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_A_7_Float = _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_RGBA_0_Vector4.a;
            float4 _Multiply_8199db1c3cb54e7696c6873206648090_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_d0bf811957fb4be4acc48ac3524321e9_Out_0_Vector4, (_SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_A_7_Float.xxxx), _Multiply_8199db1c3cb54e7696c6873206648090_Out_2_Vector4);
            UnityTexture2D _Property_1a4655242b0046a2a32fc421dafca86b_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Spot1);
            float2 _Property_100a2d69b64d4f3a8d0b5136698d86fc_Out_0_Vector2 = _Spot1Coords;
            float2 _Divide_013b956faa5b45259bd752bda3fe0620_Out_2_Vector2;
            Unity_Divide_float2(_Property_100a2d69b64d4f3a8d0b5136698d86fc_Out_0_Vector2, float2(51, 51), _Divide_013b956faa5b45259bd752bda3fe0620_Out_2_Vector2);
            UnityTexture2D _Property_28eff2915fe5433f885e119f4f163a3a_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_SpotPositions);
            float _Divide_152088a4b4f145969cb08e6782e59296_Out_2_Float;
            Unity_Divide_float(_Branch_6e389d225a3d4b4382f659549d8957bb_Out_3_Float, float(51), _Divide_152088a4b4f145969cb08e6782e59296_Out_2_Float);
            float2 _Vector2_d5bf95c8ffce4f05b08b23776ef52bfd_Out_0_Vector2 = float2(_Divide_152088a4b4f145969cb08e6782e59296_Out_2_Float, float(0));
            float4 _SampleTexture2D_f6550e316b954b469bbf81231d66f881_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_28eff2915fe5433f885e119f4f163a3a_Out_0_Texture2D.tex, _Property_28eff2915fe5433f885e119f4f163a3a_Out_0_Texture2D.samplerstate, _Property_28eff2915fe5433f885e119f4f163a3a_Out_0_Texture2D.GetTransformedUV(_Vector2_d5bf95c8ffce4f05b08b23776ef52bfd_Out_0_Vector2) );
            float _SampleTexture2D_f6550e316b954b469bbf81231d66f881_R_4_Float = _SampleTexture2D_f6550e316b954b469bbf81231d66f881_RGBA_0_Vector4.r;
            float _SampleTexture2D_f6550e316b954b469bbf81231d66f881_G_5_Float = _SampleTexture2D_f6550e316b954b469bbf81231d66f881_RGBA_0_Vector4.g;
            float _SampleTexture2D_f6550e316b954b469bbf81231d66f881_B_6_Float = _SampleTexture2D_f6550e316b954b469bbf81231d66f881_RGBA_0_Vector4.b;
            float _SampleTexture2D_f6550e316b954b469bbf81231d66f881_A_7_Float = _SampleTexture2D_f6550e316b954b469bbf81231d66f881_RGBA_0_Vector4.a;
            float2 _Vector2_27b5686b4f2641b1bacfc432832e8b14_Out_0_Vector2 = float2(_SampleTexture2D_f6550e316b954b469bbf81231d66f881_R_4_Float, _SampleTexture2D_f6550e316b954b469bbf81231d66f881_G_5_Float);
            float2 _Add_7f2d0b33a7794677b9d825d9b597f3e8_Out_2_Vector2;
            Unity_Add_float2(_Divide_013b956faa5b45259bd752bda3fe0620_Out_2_Vector2, _Vector2_27b5686b4f2641b1bacfc432832e8b14_Out_0_Vector2, _Add_7f2d0b33a7794677b9d825d9b597f3e8_Out_2_Vector2);
            float2 _Multiply_a69f7deee62e4ea2b796974ed1509798_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Add_7f2d0b33a7794677b9d825d9b597f3e8_Out_2_Vector2, float2(-1, 1), _Multiply_a69f7deee62e4ea2b796974ed1509798_Out_2_Vector2);
            float2 _TilingAndOffset_9b6b6d44caba4ff99f020f7277233356_Out_3_Vector2;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_a69f7deee62e4ea2b796974ed1509798_Out_2_Vector2, _TilingAndOffset_9b6b6d44caba4ff99f020f7277233356_Out_3_Vector2);
            float4 _SampleTexture2D_daf9019a9f404195afb964914c231dea_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_1a4655242b0046a2a32fc421dafca86b_Out_0_Texture2D.tex, _Property_1a4655242b0046a2a32fc421dafca86b_Out_0_Texture2D.samplerstate, _Property_1a4655242b0046a2a32fc421dafca86b_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_9b6b6d44caba4ff99f020f7277233356_Out_3_Vector2) );
            float _SampleTexture2D_daf9019a9f404195afb964914c231dea_R_4_Float = _SampleTexture2D_daf9019a9f404195afb964914c231dea_RGBA_0_Vector4.r;
            float _SampleTexture2D_daf9019a9f404195afb964914c231dea_G_5_Float = _SampleTexture2D_daf9019a9f404195afb964914c231dea_RGBA_0_Vector4.g;
            float _SampleTexture2D_daf9019a9f404195afb964914c231dea_B_6_Float = _SampleTexture2D_daf9019a9f404195afb964914c231dea_RGBA_0_Vector4.b;
            float _SampleTexture2D_daf9019a9f404195afb964914c231dea_A_7_Float = _SampleTexture2D_daf9019a9f404195afb964914c231dea_RGBA_0_Vector4.a;
            float4 _Multiply_4ee8cbd902954acfa842d52747aed6f2_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_daf9019a9f404195afb964914c231dea_RGBA_0_Vector4, (_SampleTexture2D_daf9019a9f404195afb964914c231dea_A_7_Float.xxxx), _Multiply_4ee8cbd902954acfa842d52747aed6f2_Out_2_Vector4);
            UnityTexture2D _Property_5ce238b92e88431cad91d2012f1babdc_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Spot2);
            float2 _Property_b34b4eb1f02040818b8a8b647abd559d_Out_0_Vector2 = _Spot2Coords;
            float2 _Divide_91e86b9383ca4be3820c03268714a476_Out_2_Vector2;
            Unity_Divide_float2(_Property_b34b4eb1f02040818b8a8b647abd559d_Out_0_Vector2, float2(51, 51), _Divide_91e86b9383ca4be3820c03268714a476_Out_2_Vector2);
            UnityTexture2D _Property_22886644eef3469f98d2e1034a092c9b_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_SpotPositions);
            float2 _Vector2_11cb4054545846d39da3453b33f152bd_Out_0_Vector2 = float2(_Divide_152088a4b4f145969cb08e6782e59296_Out_2_Float, float(0.25));
            float4 _SampleTexture2D_5618fb065a324dc99b98fcc0d91af2ba_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_22886644eef3469f98d2e1034a092c9b_Out_0_Texture2D.tex, _Property_22886644eef3469f98d2e1034a092c9b_Out_0_Texture2D.samplerstate, _Property_22886644eef3469f98d2e1034a092c9b_Out_0_Texture2D.GetTransformedUV(_Vector2_11cb4054545846d39da3453b33f152bd_Out_0_Vector2) );
            float _SampleTexture2D_5618fb065a324dc99b98fcc0d91af2ba_R_4_Float = _SampleTexture2D_5618fb065a324dc99b98fcc0d91af2ba_RGBA_0_Vector4.r;
            float _SampleTexture2D_5618fb065a324dc99b98fcc0d91af2ba_G_5_Float = _SampleTexture2D_5618fb065a324dc99b98fcc0d91af2ba_RGBA_0_Vector4.g;
            float _SampleTexture2D_5618fb065a324dc99b98fcc0d91af2ba_B_6_Float = _SampleTexture2D_5618fb065a324dc99b98fcc0d91af2ba_RGBA_0_Vector4.b;
            float _SampleTexture2D_5618fb065a324dc99b98fcc0d91af2ba_A_7_Float = _SampleTexture2D_5618fb065a324dc99b98fcc0d91af2ba_RGBA_0_Vector4.a;
            float2 _Vector2_aaa13365efd4487aba55dec191128c60_Out_0_Vector2 = float2(_SampleTexture2D_5618fb065a324dc99b98fcc0d91af2ba_R_4_Float, _SampleTexture2D_5618fb065a324dc99b98fcc0d91af2ba_G_5_Float);
            float2 _Add_bee702c296294b1d92b760361c7dabc3_Out_2_Vector2;
            Unity_Add_float2(_Divide_91e86b9383ca4be3820c03268714a476_Out_2_Vector2, _Vector2_aaa13365efd4487aba55dec191128c60_Out_0_Vector2, _Add_bee702c296294b1d92b760361c7dabc3_Out_2_Vector2);
            float2 _Multiply_159e2128caff43b0b862185320a8bd23_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Add_bee702c296294b1d92b760361c7dabc3_Out_2_Vector2, float2(-1, 1), _Multiply_159e2128caff43b0b862185320a8bd23_Out_2_Vector2);
            float2 _TilingAndOffset_194b97a9cead48f89f032d23560d4031_Out_3_Vector2;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_159e2128caff43b0b862185320a8bd23_Out_2_Vector2, _TilingAndOffset_194b97a9cead48f89f032d23560d4031_Out_3_Vector2);
            float4 _SampleTexture2D_66b19db5aa244347842e91f753da9fe0_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_5ce238b92e88431cad91d2012f1babdc_Out_0_Texture2D.tex, _Property_5ce238b92e88431cad91d2012f1babdc_Out_0_Texture2D.samplerstate, _Property_5ce238b92e88431cad91d2012f1babdc_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_194b97a9cead48f89f032d23560d4031_Out_3_Vector2) );
            float _SampleTexture2D_66b19db5aa244347842e91f753da9fe0_R_4_Float = _SampleTexture2D_66b19db5aa244347842e91f753da9fe0_RGBA_0_Vector4.r;
            float _SampleTexture2D_66b19db5aa244347842e91f753da9fe0_G_5_Float = _SampleTexture2D_66b19db5aa244347842e91f753da9fe0_RGBA_0_Vector4.g;
            float _SampleTexture2D_66b19db5aa244347842e91f753da9fe0_B_6_Float = _SampleTexture2D_66b19db5aa244347842e91f753da9fe0_RGBA_0_Vector4.b;
            float _SampleTexture2D_66b19db5aa244347842e91f753da9fe0_A_7_Float = _SampleTexture2D_66b19db5aa244347842e91f753da9fe0_RGBA_0_Vector4.a;
            float4 _Multiply_4b5972826eec42a795daaa0fb20666a7_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_66b19db5aa244347842e91f753da9fe0_RGBA_0_Vector4, (_SampleTexture2D_66b19db5aa244347842e91f753da9fe0_A_7_Float.xxxx), _Multiply_4b5972826eec42a795daaa0fb20666a7_Out_2_Vector4);
            float4 _Lerp_1ea7d711e17149d780ba160d4fa84f49_Out_3_Vector4;
            Unity_Lerp_float4(_Multiply_4ee8cbd902954acfa842d52747aed6f2_Out_2_Vector4, _Multiply_4b5972826eec42a795daaa0fb20666a7_Out_2_Vector4, (_SampleTexture2D_66b19db5aa244347842e91f753da9fe0_A_7_Float.xxxx), _Lerp_1ea7d711e17149d780ba160d4fa84f49_Out_3_Vector4);
            UnityTexture2D _Property_ce90526212064425a56a6dea840d20ad_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Spot3);
            float2 _Property_690e21fe71fc437480dedce608823a22_Out_0_Vector2 = _Spot3Coords;
            float2 _Divide_1eabd1c506e841aab79f40fbd0bc4870_Out_2_Vector2;
            Unity_Divide_float2(_Property_690e21fe71fc437480dedce608823a22_Out_0_Vector2, float2(51, 51), _Divide_1eabd1c506e841aab79f40fbd0bc4870_Out_2_Vector2);
            UnityTexture2D _Property_8b30f951ec4b4580bbb508408b626e5a_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_SpotPositions);
            float2 _Vector2_aff52d6a76b94151bc6d45bb7911b253_Out_0_Vector2 = float2(_Divide_152088a4b4f145969cb08e6782e59296_Out_2_Float, float(0.5));
            float4 _SampleTexture2D_7dccf325b07b46099d6877ecdd603bc1_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_8b30f951ec4b4580bbb508408b626e5a_Out_0_Texture2D.tex, _Property_8b30f951ec4b4580bbb508408b626e5a_Out_0_Texture2D.samplerstate, _Property_8b30f951ec4b4580bbb508408b626e5a_Out_0_Texture2D.GetTransformedUV(_Vector2_aff52d6a76b94151bc6d45bb7911b253_Out_0_Vector2) );
            float _SampleTexture2D_7dccf325b07b46099d6877ecdd603bc1_R_4_Float = _SampleTexture2D_7dccf325b07b46099d6877ecdd603bc1_RGBA_0_Vector4.r;
            float _SampleTexture2D_7dccf325b07b46099d6877ecdd603bc1_G_5_Float = _SampleTexture2D_7dccf325b07b46099d6877ecdd603bc1_RGBA_0_Vector4.g;
            float _SampleTexture2D_7dccf325b07b46099d6877ecdd603bc1_B_6_Float = _SampleTexture2D_7dccf325b07b46099d6877ecdd603bc1_RGBA_0_Vector4.b;
            float _SampleTexture2D_7dccf325b07b46099d6877ecdd603bc1_A_7_Float = _SampleTexture2D_7dccf325b07b46099d6877ecdd603bc1_RGBA_0_Vector4.a;
            float2 _Vector2_ef741e573fba4059ae514ea737e7be3e_Out_0_Vector2 = float2(_SampleTexture2D_7dccf325b07b46099d6877ecdd603bc1_R_4_Float, _SampleTexture2D_7dccf325b07b46099d6877ecdd603bc1_G_5_Float);
            float2 _Add_e1f60435d7ec45b4839b736a494ea4df_Out_2_Vector2;
            Unity_Add_float2(_Divide_1eabd1c506e841aab79f40fbd0bc4870_Out_2_Vector2, _Vector2_ef741e573fba4059ae514ea737e7be3e_Out_0_Vector2, _Add_e1f60435d7ec45b4839b736a494ea4df_Out_2_Vector2);
            float2 _Multiply_62f12fdedf2641e593ec81431de9eb86_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Add_e1f60435d7ec45b4839b736a494ea4df_Out_2_Vector2, float2(-1, 1), _Multiply_62f12fdedf2641e593ec81431de9eb86_Out_2_Vector2);
            float2 _TilingAndOffset_ae4d588d9f4944838cfa244aec873af0_Out_3_Vector2;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_62f12fdedf2641e593ec81431de9eb86_Out_2_Vector2, _TilingAndOffset_ae4d588d9f4944838cfa244aec873af0_Out_3_Vector2);
            float4 _SampleTexture2D_0f3cdcfe5f2b43459f3021be6eeef11f_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_ce90526212064425a56a6dea840d20ad_Out_0_Texture2D.tex, _Property_ce90526212064425a56a6dea840d20ad_Out_0_Texture2D.samplerstate, _Property_ce90526212064425a56a6dea840d20ad_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_ae4d588d9f4944838cfa244aec873af0_Out_3_Vector2) );
            float _SampleTexture2D_0f3cdcfe5f2b43459f3021be6eeef11f_R_4_Float = _SampleTexture2D_0f3cdcfe5f2b43459f3021be6eeef11f_RGBA_0_Vector4.r;
            float _SampleTexture2D_0f3cdcfe5f2b43459f3021be6eeef11f_G_5_Float = _SampleTexture2D_0f3cdcfe5f2b43459f3021be6eeef11f_RGBA_0_Vector4.g;
            float _SampleTexture2D_0f3cdcfe5f2b43459f3021be6eeef11f_B_6_Float = _SampleTexture2D_0f3cdcfe5f2b43459f3021be6eeef11f_RGBA_0_Vector4.b;
            float _SampleTexture2D_0f3cdcfe5f2b43459f3021be6eeef11f_A_7_Float = _SampleTexture2D_0f3cdcfe5f2b43459f3021be6eeef11f_RGBA_0_Vector4.a;
            float4 _Multiply_e3e3434263df4d5b9e5c98852fa1cb5a_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_0f3cdcfe5f2b43459f3021be6eeef11f_RGBA_0_Vector4, (_SampleTexture2D_0f3cdcfe5f2b43459f3021be6eeef11f_A_7_Float.xxxx), _Multiply_e3e3434263df4d5b9e5c98852fa1cb5a_Out_2_Vector4);
            float4 _Lerp_866ce6e94f3049ddaeddd9a05e559a3f_Out_3_Vector4;
            Unity_Lerp_float4(_Lerp_1ea7d711e17149d780ba160d4fa84f49_Out_3_Vector4, _Multiply_e3e3434263df4d5b9e5c98852fa1cb5a_Out_2_Vector4, (_SampleTexture2D_0f3cdcfe5f2b43459f3021be6eeef11f_A_7_Float.xxxx), _Lerp_866ce6e94f3049ddaeddd9a05e559a3f_Out_3_Vector4);
            UnityTexture2D _Property_235efdf0225c425a98eb7bf018650791_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Spot4);
            float2 _Property_9b30d54132ab4348ab743ef007b469eb_Out_0_Vector2 = _Spot4Coords;
            float2 _Divide_5b62c88f2abb49f7b7e76927f649c967_Out_2_Vector2;
            Unity_Divide_float2(_Property_9b30d54132ab4348ab743ef007b469eb_Out_0_Vector2, float2(51, 51), _Divide_5b62c88f2abb49f7b7e76927f649c967_Out_2_Vector2);
            UnityTexture2D _Property_a8d7f5f0852d4c98b826ecf75fda08d5_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_SpotPositions);
            float2 _Vector2_ea4b120d92404258933ac842e6c1f028_Out_0_Vector2 = float2(_Divide_152088a4b4f145969cb08e6782e59296_Out_2_Float, float(0.75));
            float4 _SampleTexture2D_0de11f9e4e3346c0acfd9d87d0bf3226_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_a8d7f5f0852d4c98b826ecf75fda08d5_Out_0_Texture2D.tex, _Property_a8d7f5f0852d4c98b826ecf75fda08d5_Out_0_Texture2D.samplerstate, _Property_a8d7f5f0852d4c98b826ecf75fda08d5_Out_0_Texture2D.GetTransformedUV(_Vector2_ea4b120d92404258933ac842e6c1f028_Out_0_Vector2) );
            float _SampleTexture2D_0de11f9e4e3346c0acfd9d87d0bf3226_R_4_Float = _SampleTexture2D_0de11f9e4e3346c0acfd9d87d0bf3226_RGBA_0_Vector4.r;
            float _SampleTexture2D_0de11f9e4e3346c0acfd9d87d0bf3226_G_5_Float = _SampleTexture2D_0de11f9e4e3346c0acfd9d87d0bf3226_RGBA_0_Vector4.g;
            float _SampleTexture2D_0de11f9e4e3346c0acfd9d87d0bf3226_B_6_Float = _SampleTexture2D_0de11f9e4e3346c0acfd9d87d0bf3226_RGBA_0_Vector4.b;
            float _SampleTexture2D_0de11f9e4e3346c0acfd9d87d0bf3226_A_7_Float = _SampleTexture2D_0de11f9e4e3346c0acfd9d87d0bf3226_RGBA_0_Vector4.a;
            float2 _Vector2_e57c57df21584697b1e53df3e8fdef5f_Out_0_Vector2 = float2(_SampleTexture2D_0de11f9e4e3346c0acfd9d87d0bf3226_R_4_Float, _SampleTexture2D_0de11f9e4e3346c0acfd9d87d0bf3226_G_5_Float);
            float2 _Add_9a17f07b62e841d4956243c6db5c8b8c_Out_2_Vector2;
            Unity_Add_float2(_Divide_5b62c88f2abb49f7b7e76927f649c967_Out_2_Vector2, _Vector2_e57c57df21584697b1e53df3e8fdef5f_Out_0_Vector2, _Add_9a17f07b62e841d4956243c6db5c8b8c_Out_2_Vector2);
            float2 _Multiply_e326edeede824a75a7582cbf5ebed0d3_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Add_9a17f07b62e841d4956243c6db5c8b8c_Out_2_Vector2, float2(-1, 1), _Multiply_e326edeede824a75a7582cbf5ebed0d3_Out_2_Vector2);
            float2 _TilingAndOffset_df92dd591b0a478aa5d05850a316a082_Out_3_Vector2;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_e326edeede824a75a7582cbf5ebed0d3_Out_2_Vector2, _TilingAndOffset_df92dd591b0a478aa5d05850a316a082_Out_3_Vector2);
            float4 _SampleTexture2D_2b2329f3587343fb9b1354d0a6e7531c_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_235efdf0225c425a98eb7bf018650791_Out_0_Texture2D.tex, _Property_235efdf0225c425a98eb7bf018650791_Out_0_Texture2D.samplerstate, _Property_235efdf0225c425a98eb7bf018650791_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_df92dd591b0a478aa5d05850a316a082_Out_3_Vector2) );
            float _SampleTexture2D_2b2329f3587343fb9b1354d0a6e7531c_R_4_Float = _SampleTexture2D_2b2329f3587343fb9b1354d0a6e7531c_RGBA_0_Vector4.r;
            float _SampleTexture2D_2b2329f3587343fb9b1354d0a6e7531c_G_5_Float = _SampleTexture2D_2b2329f3587343fb9b1354d0a6e7531c_RGBA_0_Vector4.g;
            float _SampleTexture2D_2b2329f3587343fb9b1354d0a6e7531c_B_6_Float = _SampleTexture2D_2b2329f3587343fb9b1354d0a6e7531c_RGBA_0_Vector4.b;
            float _SampleTexture2D_2b2329f3587343fb9b1354d0a6e7531c_A_7_Float = _SampleTexture2D_2b2329f3587343fb9b1354d0a6e7531c_RGBA_0_Vector4.a;
            float4 _Multiply_956573eb07ff40aebdd30c3b33c53851_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_2b2329f3587343fb9b1354d0a6e7531c_RGBA_0_Vector4, (_SampleTexture2D_2b2329f3587343fb9b1354d0a6e7531c_A_7_Float.xxxx), _Multiply_956573eb07ff40aebdd30c3b33c53851_Out_2_Vector4);
            float4 _Lerp_86cae1c94e4146beb4d6b43ce0b16b6f_Out_3_Vector4;
            Unity_Lerp_float4(_Lerp_866ce6e94f3049ddaeddd9a05e559a3f_Out_3_Vector4, _Multiply_956573eb07ff40aebdd30c3b33c53851_Out_2_Vector4, (_SampleTexture2D_2b2329f3587343fb9b1354d0a6e7531c_A_7_Float.xxxx), _Lerp_86cae1c94e4146beb4d6b43ce0b16b6f_Out_3_Vector4);
            float _Maximum_58dfe2c80609413f93b66794c0dc58a2_Out_2_Float;
            Unity_Maximum_float(_SampleTexture2D_daf9019a9f404195afb964914c231dea_A_7_Float, _SampleTexture2D_66b19db5aa244347842e91f753da9fe0_A_7_Float, _Maximum_58dfe2c80609413f93b66794c0dc58a2_Out_2_Float);
            float _Maximum_19d1234cbbf243519e9851cb3e5ac03d_Out_2_Float;
            Unity_Maximum_float(_Maximum_58dfe2c80609413f93b66794c0dc58a2_Out_2_Float, _SampleTexture2D_0f3cdcfe5f2b43459f3021be6eeef11f_A_7_Float, _Maximum_19d1234cbbf243519e9851cb3e5ac03d_Out_2_Float);
            float _Maximum_2a4c60d3d91045bab963f0bd1234e469_Out_2_Float;
            Unity_Maximum_float(_Maximum_19d1234cbbf243519e9851cb3e5ac03d_Out_2_Float, _SampleTexture2D_2b2329f3587343fb9b1354d0a6e7531c_A_7_Float, _Maximum_2a4c60d3d91045bab963f0bd1234e469_Out_2_Float);
            float4 _Blend_63db57ca102743e0ba2c6fe8e093e384_Out_2_Vector4;
            Unity_Blend_Multiply_float4(_SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_RGBA_0_Vector4, _Lerp_86cae1c94e4146beb4d6b43ce0b16b6f_Out_3_Vector4, _Blend_63db57ca102743e0ba2c6fe8e093e384_Out_2_Vector4, _Maximum_2a4c60d3d91045bab963f0bd1234e469_Out_2_Float);
            float4 _Preview_fe039e510ff94946ac700f7155669a9f_Out_1_Vector4;
            Unity_Preview_float4(_Blend_63db57ca102743e0ba2c6fe8e093e384_Out_2_Vector4, _Preview_fe039e510ff94946ac700f7155669a9f_Out_1_Vector4);
            float4 _Branch_4d8551bfe3f449c2ac8e679a439bd984_Out_3_Vector4;
            Unity_Branch_float4(_Property_d33558c987894df9bf624c76589f84ee_Out_0_Boolean, _Multiply_8199db1c3cb54e7696c6873206648090_Out_2_Vector4, _Preview_fe039e510ff94946ac700f7155669a9f_Out_1_Vector4, _Branch_4d8551bfe3f449c2ac8e679a439bd984_Out_3_Vector4);
            float _Property_ef95125fd9cd422a85117efeb52dcda4_Out_0_Float = _ShadowAlpha;
            float _Branch_b6fb662ef570428baf596a92fa3606fe_Out_3_Float;
            Unity_Branch_float(_Property_d33558c987894df9bf624c76589f84ee_Out_0_Boolean, _Property_ef95125fd9cd422a85117efeb52dcda4_Out_0_Float, float(1), _Branch_b6fb662ef570428baf596a92fa3606fe_Out_3_Float);
            float _Preview_f8a06c9fecd64f90a5b3f436d754997b_Out_1_Float;
            Unity_Preview_float(_SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_A_7_Float, _Preview_f8a06c9fecd64f90a5b3f436d754997b_Out_1_Float);
            float _Multiply_6c102e2410a94b0e935f420646e13837_Out_2_Float;
            Unity_Multiply_float_float(_Branch_b6fb662ef570428baf596a92fa3606fe_Out_3_Float, _Preview_f8a06c9fecd64f90a5b3f436d754997b_Out_1_Float, _Multiply_6c102e2410a94b0e935f420646e13837_Out_2_Float);
            surface.BaseColor = (_Branch_4d8551bfe3f449c2ac8e679a439bd984_Out_3_Vector4.xyz);
            surface.Alpha = _Multiply_6c102e2410a94b0e935f420646e13837_Out_2_Float;
            surface.AlphaClipThreshold = float(0.5);
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENESELECTIONPASS 1
        
        #define _ALPHATEST_ON 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _SpriteSheet_TexelSize;
        float4 _SpriteSheet_ST;
        float2 _Flipbook;
        float _Speed;
        float4 _MainTex_TexelSize;
        float4 _ShadowColor;
        float _IsShadow;
        float _ShadowAlpha;
        float _AutoAnimate;
        float _SpriteIndex;
        float _MaxIndex;
        float _SpecificIndex;
        float _MinIndex;
        float2 _Spot1Coords;
        float2 _Spot2Coords;
        float2 _Spot3Coords;
        float2 _Spot4Coords;
        float4 _Spot1_TexelSize;
        float4 _Spot2_TexelSize;
        float4 _Spot3_TexelSize;
        float4 _Spot4_TexelSize;
        float4 _SpotPositions_TexelSize;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SpriteSheet);
        SAMPLER(sampler_SpriteSheet);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_Spot1);
        SAMPLER(sampler_Spot1);
        TEXTURE2D(_Spot2);
        SAMPLER(sampler_Spot2);
        TEXTURE2D(_Spot3);
        SAMPLER(sampler_Spot3);
        TEXTURE2D(_Spot4);
        SAMPLER(sampler_Spot4);
        TEXTURE2D(_SpotPositions);
        SAMPLER(sampler_SpotPositions);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Floor_float(float In, out float Out)
        {
            Out = floor(In);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Modulo_float(float A, float B, out float Out)
        {
            Out = fmod(A, B);
        }
        
        void Unity_Flipbook_InvertY_float (float2 UV, float Width, float Height, float Tile, float2 Invert, out float2 Out)
        {
            Tile = floor(fmod(Tile + float(0.00001), Width*Height));
            float2 tileCount = float2(1.0, 1.0) / float2(Width, Height);
            float base = floor((Tile + float(0.5)) * tileCount.x);
            float tileX = (Tile - Width * base);
            float tileY = (Invert.y * Height - (base + Invert.y * 1));
            Out = (UV + float2(tileX, tileY)) * tileCount;
        }
        
        void Unity_Preview_float(float In, out float Out)
        {
            Out = In;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_d33558c987894df9bf624c76589f84ee_Out_0_Boolean = _IsShadow;
            float _Property_ef95125fd9cd422a85117efeb52dcda4_Out_0_Float = _ShadowAlpha;
            float _Branch_b6fb662ef570428baf596a92fa3606fe_Out_3_Float;
            Unity_Branch_float(_Property_d33558c987894df9bf624c76589f84ee_Out_0_Boolean, _Property_ef95125fd9cd422a85117efeb52dcda4_Out_0_Float, float(1), _Branch_b6fb662ef570428baf596a92fa3606fe_Out_3_Float);
            UnityTexture2D _Property_97fd4aa82e05435894a5923b475c61fd_Out_0_Texture2D = UnityBuildTexture2DStruct(_SpriteSheet);
            float2 _Property_a9897e89a2234f828d69cd756b02f8ec_Out_0_Vector2 = _Flipbook;
            float _Split_3d7acd9d31ed4dc791759fa0f2ba28e2_R_1_Float = _Property_a9897e89a2234f828d69cd756b02f8ec_Out_0_Vector2[0];
            float _Split_3d7acd9d31ed4dc791759fa0f2ba28e2_G_2_Float = _Property_a9897e89a2234f828d69cd756b02f8ec_Out_0_Vector2[1];
            float _Split_3d7acd9d31ed4dc791759fa0f2ba28e2_B_3_Float = 0;
            float _Split_3d7acd9d31ed4dc791759fa0f2ba28e2_A_4_Float = 0;
            float _Property_df4ef9ac52f54ff891606e4915a58b47_Out_0_Boolean = _AutoAnimate;
            float _Property_ceb17021a85a40469f6d4da63e231c35_Out_0_Boolean = _SpecificIndex;
            float _Property_42cb7d43d6144dd38ec1c722c9bda400_Out_0_Float = _Speed;
            float _Multiply_0dcaf02843384467875717b4b0671a50_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_42cb7d43d6144dd38ec1c722c9bda400_Out_0_Float, _Multiply_0dcaf02843384467875717b4b0671a50_Out_2_Float);
            float _Floor_8be86eeacaf94b0db24577bcbfcdbdfd_Out_1_Float;
            Unity_Floor_float(_Multiply_0dcaf02843384467875717b4b0671a50_Out_2_Float, _Floor_8be86eeacaf94b0db24577bcbfcdbdfd_Out_1_Float);
            float _Property_9e5c02c283504d1caca6e793828cb16b_Out_0_Float = _MinIndex;
            float _Add_751390100f9e49ebaa806f60f32d70c0_Out_2_Float;
            Unity_Add_float(_Floor_8be86eeacaf94b0db24577bcbfcdbdfd_Out_1_Float, _Property_9e5c02c283504d1caca6e793828cb16b_Out_0_Float, _Add_751390100f9e49ebaa806f60f32d70c0_Out_2_Float);
            float _Property_855ccdcd2d34412aba0ac01bb659ec4d_Out_0_Float = _MaxIndex;
            float _Subtract_e917f3dbe48740ce818c535ba035c918_Out_2_Float;
            Unity_Subtract_float(_Property_855ccdcd2d34412aba0ac01bb659ec4d_Out_0_Float, _Property_9e5c02c283504d1caca6e793828cb16b_Out_0_Float, _Subtract_e917f3dbe48740ce818c535ba035c918_Out_2_Float);
            float _Modulo_7b3bd6bae30147b688a716ebf2bc8092_Out_2_Float;
            Unity_Modulo_float(_Add_751390100f9e49ebaa806f60f32d70c0_Out_2_Float, _Subtract_e917f3dbe48740ce818c535ba035c918_Out_2_Float, _Modulo_7b3bd6bae30147b688a716ebf2bc8092_Out_2_Float);
            float _Branch_d447bbc59ef6419a8bf995775d5e6e72_Out_3_Float;
            Unity_Branch_float(_Property_ceb17021a85a40469f6d4da63e231c35_Out_0_Boolean, _Modulo_7b3bd6bae30147b688a716ebf2bc8092_Out_2_Float, _Floor_8be86eeacaf94b0db24577bcbfcdbdfd_Out_1_Float, _Branch_d447bbc59ef6419a8bf995775d5e6e72_Out_3_Float);
            float _Property_89da1bdd17c64d2cad478880089d481b_Out_0_Float = _SpriteIndex;
            float _Branch_6e389d225a3d4b4382f659549d8957bb_Out_3_Float;
            Unity_Branch_float(_Property_df4ef9ac52f54ff891606e4915a58b47_Out_0_Boolean, _Branch_d447bbc59ef6419a8bf995775d5e6e72_Out_3_Float, _Property_89da1bdd17c64d2cad478880089d481b_Out_0_Float, _Branch_6e389d225a3d4b4382f659549d8957bb_Out_3_Float);
            float2 _Flipbook_cf3c7b4cd4834ca7baf944c89ae7e107_Out_4_Vector2;
            float2 _Flipbook_cf3c7b4cd4834ca7baf944c89ae7e107_Invert = float2 (0, 1);
            Unity_Flipbook_InvertY_float(IN.uv0.xy, _Split_3d7acd9d31ed4dc791759fa0f2ba28e2_R_1_Float, _Split_3d7acd9d31ed4dc791759fa0f2ba28e2_G_2_Float, _Branch_6e389d225a3d4b4382f659549d8957bb_Out_3_Float, _Flipbook_cf3c7b4cd4834ca7baf944c89ae7e107_Invert, _Flipbook_cf3c7b4cd4834ca7baf944c89ae7e107_Out_4_Vector2);
            float4 _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_97fd4aa82e05435894a5923b475c61fd_Out_0_Texture2D.tex, _Property_97fd4aa82e05435894a5923b475c61fd_Out_0_Texture2D.samplerstate, _Property_97fd4aa82e05435894a5923b475c61fd_Out_0_Texture2D.GetTransformedUV(_Flipbook_cf3c7b4cd4834ca7baf944c89ae7e107_Out_4_Vector2) );
            float _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_R_4_Float = _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_RGBA_0_Vector4.r;
            float _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_G_5_Float = _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_RGBA_0_Vector4.g;
            float _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_B_6_Float = _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_RGBA_0_Vector4.b;
            float _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_A_7_Float = _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_RGBA_0_Vector4.a;
            float _Preview_f8a06c9fecd64f90a5b3f436d754997b_Out_1_Float;
            Unity_Preview_float(_SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_A_7_Float, _Preview_f8a06c9fecd64f90a5b3f436d754997b_Out_1_Float);
            float _Multiply_6c102e2410a94b0e935f420646e13837_Out_2_Float;
            Unity_Multiply_float_float(_Branch_b6fb662ef570428baf596a92fa3606fe_Out_3_Float, _Preview_f8a06c9fecd64f90a5b3f436d754997b_Out_1_Float, _Multiply_6c102e2410a94b0e935f420646e13837_Out_2_Float);
            surface.Alpha = _Multiply_6c102e2410a94b0e935f420646e13837_Out_2_Float;
            surface.AlphaClipThreshold = float(0.5);
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }
        
        // Render State
        Cull Back
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENEPICKINGPASS 1
        
        #define _ALPHATEST_ON 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _SpriteSheet_TexelSize;
        float4 _SpriteSheet_ST;
        float2 _Flipbook;
        float _Speed;
        float4 _MainTex_TexelSize;
        float4 _ShadowColor;
        float _IsShadow;
        float _ShadowAlpha;
        float _AutoAnimate;
        float _SpriteIndex;
        float _MaxIndex;
        float _SpecificIndex;
        float _MinIndex;
        float2 _Spot1Coords;
        float2 _Spot2Coords;
        float2 _Spot3Coords;
        float2 _Spot4Coords;
        float4 _Spot1_TexelSize;
        float4 _Spot2_TexelSize;
        float4 _Spot3_TexelSize;
        float4 _Spot4_TexelSize;
        float4 _SpotPositions_TexelSize;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SpriteSheet);
        SAMPLER(sampler_SpriteSheet);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_Spot1);
        SAMPLER(sampler_Spot1);
        TEXTURE2D(_Spot2);
        SAMPLER(sampler_Spot2);
        TEXTURE2D(_Spot3);
        SAMPLER(sampler_Spot3);
        TEXTURE2D(_Spot4);
        SAMPLER(sampler_Spot4);
        TEXTURE2D(_SpotPositions);
        SAMPLER(sampler_SpotPositions);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Floor_float(float In, out float Out)
        {
            Out = floor(In);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Modulo_float(float A, float B, out float Out)
        {
            Out = fmod(A, B);
        }
        
        void Unity_Flipbook_InvertY_float (float2 UV, float Width, float Height, float Tile, float2 Invert, out float2 Out)
        {
            Tile = floor(fmod(Tile + float(0.00001), Width*Height));
            float2 tileCount = float2(1.0, 1.0) / float2(Width, Height);
            float base = floor((Tile + float(0.5)) * tileCount.x);
            float tileX = (Tile - Width * base);
            float tileY = (Invert.y * Height - (base + Invert.y * 1));
            Out = (UV + float2(tileX, tileY)) * tileCount;
        }
        
        void Unity_Preview_float(float In, out float Out)
        {
            Out = In;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_d33558c987894df9bf624c76589f84ee_Out_0_Boolean = _IsShadow;
            float _Property_ef95125fd9cd422a85117efeb52dcda4_Out_0_Float = _ShadowAlpha;
            float _Branch_b6fb662ef570428baf596a92fa3606fe_Out_3_Float;
            Unity_Branch_float(_Property_d33558c987894df9bf624c76589f84ee_Out_0_Boolean, _Property_ef95125fd9cd422a85117efeb52dcda4_Out_0_Float, float(1), _Branch_b6fb662ef570428baf596a92fa3606fe_Out_3_Float);
            UnityTexture2D _Property_97fd4aa82e05435894a5923b475c61fd_Out_0_Texture2D = UnityBuildTexture2DStruct(_SpriteSheet);
            float2 _Property_a9897e89a2234f828d69cd756b02f8ec_Out_0_Vector2 = _Flipbook;
            float _Split_3d7acd9d31ed4dc791759fa0f2ba28e2_R_1_Float = _Property_a9897e89a2234f828d69cd756b02f8ec_Out_0_Vector2[0];
            float _Split_3d7acd9d31ed4dc791759fa0f2ba28e2_G_2_Float = _Property_a9897e89a2234f828d69cd756b02f8ec_Out_0_Vector2[1];
            float _Split_3d7acd9d31ed4dc791759fa0f2ba28e2_B_3_Float = 0;
            float _Split_3d7acd9d31ed4dc791759fa0f2ba28e2_A_4_Float = 0;
            float _Property_df4ef9ac52f54ff891606e4915a58b47_Out_0_Boolean = _AutoAnimate;
            float _Property_ceb17021a85a40469f6d4da63e231c35_Out_0_Boolean = _SpecificIndex;
            float _Property_42cb7d43d6144dd38ec1c722c9bda400_Out_0_Float = _Speed;
            float _Multiply_0dcaf02843384467875717b4b0671a50_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_42cb7d43d6144dd38ec1c722c9bda400_Out_0_Float, _Multiply_0dcaf02843384467875717b4b0671a50_Out_2_Float);
            float _Floor_8be86eeacaf94b0db24577bcbfcdbdfd_Out_1_Float;
            Unity_Floor_float(_Multiply_0dcaf02843384467875717b4b0671a50_Out_2_Float, _Floor_8be86eeacaf94b0db24577bcbfcdbdfd_Out_1_Float);
            float _Property_9e5c02c283504d1caca6e793828cb16b_Out_0_Float = _MinIndex;
            float _Add_751390100f9e49ebaa806f60f32d70c0_Out_2_Float;
            Unity_Add_float(_Floor_8be86eeacaf94b0db24577bcbfcdbdfd_Out_1_Float, _Property_9e5c02c283504d1caca6e793828cb16b_Out_0_Float, _Add_751390100f9e49ebaa806f60f32d70c0_Out_2_Float);
            float _Property_855ccdcd2d34412aba0ac01bb659ec4d_Out_0_Float = _MaxIndex;
            float _Subtract_e917f3dbe48740ce818c535ba035c918_Out_2_Float;
            Unity_Subtract_float(_Property_855ccdcd2d34412aba0ac01bb659ec4d_Out_0_Float, _Property_9e5c02c283504d1caca6e793828cb16b_Out_0_Float, _Subtract_e917f3dbe48740ce818c535ba035c918_Out_2_Float);
            float _Modulo_7b3bd6bae30147b688a716ebf2bc8092_Out_2_Float;
            Unity_Modulo_float(_Add_751390100f9e49ebaa806f60f32d70c0_Out_2_Float, _Subtract_e917f3dbe48740ce818c535ba035c918_Out_2_Float, _Modulo_7b3bd6bae30147b688a716ebf2bc8092_Out_2_Float);
            float _Branch_d447bbc59ef6419a8bf995775d5e6e72_Out_3_Float;
            Unity_Branch_float(_Property_ceb17021a85a40469f6d4da63e231c35_Out_0_Boolean, _Modulo_7b3bd6bae30147b688a716ebf2bc8092_Out_2_Float, _Floor_8be86eeacaf94b0db24577bcbfcdbdfd_Out_1_Float, _Branch_d447bbc59ef6419a8bf995775d5e6e72_Out_3_Float);
            float _Property_89da1bdd17c64d2cad478880089d481b_Out_0_Float = _SpriteIndex;
            float _Branch_6e389d225a3d4b4382f659549d8957bb_Out_3_Float;
            Unity_Branch_float(_Property_df4ef9ac52f54ff891606e4915a58b47_Out_0_Boolean, _Branch_d447bbc59ef6419a8bf995775d5e6e72_Out_3_Float, _Property_89da1bdd17c64d2cad478880089d481b_Out_0_Float, _Branch_6e389d225a3d4b4382f659549d8957bb_Out_3_Float);
            float2 _Flipbook_cf3c7b4cd4834ca7baf944c89ae7e107_Out_4_Vector2;
            float2 _Flipbook_cf3c7b4cd4834ca7baf944c89ae7e107_Invert = float2 (0, 1);
            Unity_Flipbook_InvertY_float(IN.uv0.xy, _Split_3d7acd9d31ed4dc791759fa0f2ba28e2_R_1_Float, _Split_3d7acd9d31ed4dc791759fa0f2ba28e2_G_2_Float, _Branch_6e389d225a3d4b4382f659549d8957bb_Out_3_Float, _Flipbook_cf3c7b4cd4834ca7baf944c89ae7e107_Invert, _Flipbook_cf3c7b4cd4834ca7baf944c89ae7e107_Out_4_Vector2);
            float4 _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_97fd4aa82e05435894a5923b475c61fd_Out_0_Texture2D.tex, _Property_97fd4aa82e05435894a5923b475c61fd_Out_0_Texture2D.samplerstate, _Property_97fd4aa82e05435894a5923b475c61fd_Out_0_Texture2D.GetTransformedUV(_Flipbook_cf3c7b4cd4834ca7baf944c89ae7e107_Out_4_Vector2) );
            float _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_R_4_Float = _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_RGBA_0_Vector4.r;
            float _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_G_5_Float = _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_RGBA_0_Vector4.g;
            float _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_B_6_Float = _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_RGBA_0_Vector4.b;
            float _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_A_7_Float = _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_RGBA_0_Vector4.a;
            float _Preview_f8a06c9fecd64f90a5b3f436d754997b_Out_1_Float;
            Unity_Preview_float(_SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_A_7_Float, _Preview_f8a06c9fecd64f90a5b3f436d754997b_Out_1_Float);
            float _Multiply_6c102e2410a94b0e935f420646e13837_Out_2_Float;
            Unity_Multiply_float_float(_Branch_b6fb662ef570428baf596a92fa3606fe_Out_3_Float, _Preview_f8a06c9fecd64f90a5b3f436d754997b_Out_1_Float, _Multiply_6c102e2410a94b0e935f420646e13837_Out_2_Float);
            surface.Alpha = _Multiply_6c102e2410a94b0e935f420646e13837_Out_2_Float;
            surface.AlphaClipThreshold = float(0.5);
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
        
        // Render State
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEFORWARD
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 color : INTERP1;
             float3 positionWS : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.color.xyzw = input.color;
            output.positionWS.xyz = input.positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.color = input.color.xyzw;
            output.positionWS = input.positionWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _SpriteSheet_TexelSize;
        float4 _SpriteSheet_ST;
        float2 _Flipbook;
        float _Speed;
        float4 _MainTex_TexelSize;
        float4 _ShadowColor;
        float _IsShadow;
        float _ShadowAlpha;
        float _AutoAnimate;
        float _SpriteIndex;
        float _MaxIndex;
        float _SpecificIndex;
        float _MinIndex;
        float2 _Spot1Coords;
        float2 _Spot2Coords;
        float2 _Spot3Coords;
        float2 _Spot4Coords;
        float4 _Spot1_TexelSize;
        float4 _Spot2_TexelSize;
        float4 _Spot3_TexelSize;
        float4 _Spot4_TexelSize;
        float4 _SpotPositions_TexelSize;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SpriteSheet);
        SAMPLER(sampler_SpriteSheet);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_Spot1);
        SAMPLER(sampler_Spot1);
        TEXTURE2D(_Spot2);
        SAMPLER(sampler_Spot2);
        TEXTURE2D(_Spot3);
        SAMPLER(sampler_Spot3);
        TEXTURE2D(_Spot4);
        SAMPLER(sampler_Spot4);
        TEXTURE2D(_SpotPositions);
        SAMPLER(sampler_SpotPositions);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Floor_float(float In, out float Out)
        {
            Out = floor(In);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Modulo_float(float A, float B, out float Out)
        {
            Out = fmod(A, B);
        }
        
        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_Flipbook_InvertY_float (float2 UV, float Width, float Height, float Tile, float2 Invert, out float2 Out)
        {
            Tile = floor(fmod(Tile + float(0.00001), Width*Height));
            float2 tileCount = float2(1.0, 1.0) / float2(Width, Height);
            float base = floor((Tile + float(0.5)) * tileCount.x);
            float tileX = (Tile - Width * base);
            float tileY = (Invert.y * Height - (base + Invert.y * 1));
            Out = (UV + float2(tileX, tileY)) * tileCount;
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Maximum_float(float A, float B, out float Out)
        {
            Out = max(A, B);
        }
        
        void Unity_Blend_Multiply_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
        {
            Out = Base * Blend;
            Out = lerp(Base, Out, Opacity);
        }
        
        void Unity_Preview_float4(float4 In, out float4 Out)
        {
            Out = In;
        }
        
        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_Preview_float(float In, out float Out)
        {
            Out = In;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_d33558c987894df9bf624c76589f84ee_Out_0_Boolean = _IsShadow;
            float4 _Property_d0bf811957fb4be4acc48ac3524321e9_Out_0_Vector4 = _ShadowColor;
            UnityTexture2D _Property_97fd4aa82e05435894a5923b475c61fd_Out_0_Texture2D = UnityBuildTexture2DStruct(_SpriteSheet);
            float2 _Property_a9897e89a2234f828d69cd756b02f8ec_Out_0_Vector2 = _Flipbook;
            float _Split_3d7acd9d31ed4dc791759fa0f2ba28e2_R_1_Float = _Property_a9897e89a2234f828d69cd756b02f8ec_Out_0_Vector2[0];
            float _Split_3d7acd9d31ed4dc791759fa0f2ba28e2_G_2_Float = _Property_a9897e89a2234f828d69cd756b02f8ec_Out_0_Vector2[1];
            float _Split_3d7acd9d31ed4dc791759fa0f2ba28e2_B_3_Float = 0;
            float _Split_3d7acd9d31ed4dc791759fa0f2ba28e2_A_4_Float = 0;
            float _Property_df4ef9ac52f54ff891606e4915a58b47_Out_0_Boolean = _AutoAnimate;
            float _Property_ceb17021a85a40469f6d4da63e231c35_Out_0_Boolean = _SpecificIndex;
            float _Property_42cb7d43d6144dd38ec1c722c9bda400_Out_0_Float = _Speed;
            float _Multiply_0dcaf02843384467875717b4b0671a50_Out_2_Float;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_42cb7d43d6144dd38ec1c722c9bda400_Out_0_Float, _Multiply_0dcaf02843384467875717b4b0671a50_Out_2_Float);
            float _Floor_8be86eeacaf94b0db24577bcbfcdbdfd_Out_1_Float;
            Unity_Floor_float(_Multiply_0dcaf02843384467875717b4b0671a50_Out_2_Float, _Floor_8be86eeacaf94b0db24577bcbfcdbdfd_Out_1_Float);
            float _Property_9e5c02c283504d1caca6e793828cb16b_Out_0_Float = _MinIndex;
            float _Add_751390100f9e49ebaa806f60f32d70c0_Out_2_Float;
            Unity_Add_float(_Floor_8be86eeacaf94b0db24577bcbfcdbdfd_Out_1_Float, _Property_9e5c02c283504d1caca6e793828cb16b_Out_0_Float, _Add_751390100f9e49ebaa806f60f32d70c0_Out_2_Float);
            float _Property_855ccdcd2d34412aba0ac01bb659ec4d_Out_0_Float = _MaxIndex;
            float _Subtract_e917f3dbe48740ce818c535ba035c918_Out_2_Float;
            Unity_Subtract_float(_Property_855ccdcd2d34412aba0ac01bb659ec4d_Out_0_Float, _Property_9e5c02c283504d1caca6e793828cb16b_Out_0_Float, _Subtract_e917f3dbe48740ce818c535ba035c918_Out_2_Float);
            float _Modulo_7b3bd6bae30147b688a716ebf2bc8092_Out_2_Float;
            Unity_Modulo_float(_Add_751390100f9e49ebaa806f60f32d70c0_Out_2_Float, _Subtract_e917f3dbe48740ce818c535ba035c918_Out_2_Float, _Modulo_7b3bd6bae30147b688a716ebf2bc8092_Out_2_Float);
            float _Branch_d447bbc59ef6419a8bf995775d5e6e72_Out_3_Float;
            Unity_Branch_float(_Property_ceb17021a85a40469f6d4da63e231c35_Out_0_Boolean, _Modulo_7b3bd6bae30147b688a716ebf2bc8092_Out_2_Float, _Floor_8be86eeacaf94b0db24577bcbfcdbdfd_Out_1_Float, _Branch_d447bbc59ef6419a8bf995775d5e6e72_Out_3_Float);
            float _Property_89da1bdd17c64d2cad478880089d481b_Out_0_Float = _SpriteIndex;
            float _Branch_6e389d225a3d4b4382f659549d8957bb_Out_3_Float;
            Unity_Branch_float(_Property_df4ef9ac52f54ff891606e4915a58b47_Out_0_Boolean, _Branch_d447bbc59ef6419a8bf995775d5e6e72_Out_3_Float, _Property_89da1bdd17c64d2cad478880089d481b_Out_0_Float, _Branch_6e389d225a3d4b4382f659549d8957bb_Out_3_Float);
            float2 _Flipbook_cf3c7b4cd4834ca7baf944c89ae7e107_Out_4_Vector2;
            float2 _Flipbook_cf3c7b4cd4834ca7baf944c89ae7e107_Invert = float2 (0, 1);
            Unity_Flipbook_InvertY_float(IN.uv0.xy, _Split_3d7acd9d31ed4dc791759fa0f2ba28e2_R_1_Float, _Split_3d7acd9d31ed4dc791759fa0f2ba28e2_G_2_Float, _Branch_6e389d225a3d4b4382f659549d8957bb_Out_3_Float, _Flipbook_cf3c7b4cd4834ca7baf944c89ae7e107_Invert, _Flipbook_cf3c7b4cd4834ca7baf944c89ae7e107_Out_4_Vector2);
            float4 _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_97fd4aa82e05435894a5923b475c61fd_Out_0_Texture2D.tex, _Property_97fd4aa82e05435894a5923b475c61fd_Out_0_Texture2D.samplerstate, _Property_97fd4aa82e05435894a5923b475c61fd_Out_0_Texture2D.GetTransformedUV(_Flipbook_cf3c7b4cd4834ca7baf944c89ae7e107_Out_4_Vector2) );
            float _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_R_4_Float = _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_RGBA_0_Vector4.r;
            float _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_G_5_Float = _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_RGBA_0_Vector4.g;
            float _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_B_6_Float = _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_RGBA_0_Vector4.b;
            float _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_A_7_Float = _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_RGBA_0_Vector4.a;
            float4 _Multiply_8199db1c3cb54e7696c6873206648090_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_d0bf811957fb4be4acc48ac3524321e9_Out_0_Vector4, (_SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_A_7_Float.xxxx), _Multiply_8199db1c3cb54e7696c6873206648090_Out_2_Vector4);
            UnityTexture2D _Property_1a4655242b0046a2a32fc421dafca86b_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Spot1);
            float2 _Property_100a2d69b64d4f3a8d0b5136698d86fc_Out_0_Vector2 = _Spot1Coords;
            float2 _Divide_013b956faa5b45259bd752bda3fe0620_Out_2_Vector2;
            Unity_Divide_float2(_Property_100a2d69b64d4f3a8d0b5136698d86fc_Out_0_Vector2, float2(51, 51), _Divide_013b956faa5b45259bd752bda3fe0620_Out_2_Vector2);
            UnityTexture2D _Property_28eff2915fe5433f885e119f4f163a3a_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_SpotPositions);
            float _Divide_152088a4b4f145969cb08e6782e59296_Out_2_Float;
            Unity_Divide_float(_Branch_6e389d225a3d4b4382f659549d8957bb_Out_3_Float, float(51), _Divide_152088a4b4f145969cb08e6782e59296_Out_2_Float);
            float2 _Vector2_d5bf95c8ffce4f05b08b23776ef52bfd_Out_0_Vector2 = float2(_Divide_152088a4b4f145969cb08e6782e59296_Out_2_Float, float(0));
            float4 _SampleTexture2D_f6550e316b954b469bbf81231d66f881_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_28eff2915fe5433f885e119f4f163a3a_Out_0_Texture2D.tex, _Property_28eff2915fe5433f885e119f4f163a3a_Out_0_Texture2D.samplerstate, _Property_28eff2915fe5433f885e119f4f163a3a_Out_0_Texture2D.GetTransformedUV(_Vector2_d5bf95c8ffce4f05b08b23776ef52bfd_Out_0_Vector2) );
            float _SampleTexture2D_f6550e316b954b469bbf81231d66f881_R_4_Float = _SampleTexture2D_f6550e316b954b469bbf81231d66f881_RGBA_0_Vector4.r;
            float _SampleTexture2D_f6550e316b954b469bbf81231d66f881_G_5_Float = _SampleTexture2D_f6550e316b954b469bbf81231d66f881_RGBA_0_Vector4.g;
            float _SampleTexture2D_f6550e316b954b469bbf81231d66f881_B_6_Float = _SampleTexture2D_f6550e316b954b469bbf81231d66f881_RGBA_0_Vector4.b;
            float _SampleTexture2D_f6550e316b954b469bbf81231d66f881_A_7_Float = _SampleTexture2D_f6550e316b954b469bbf81231d66f881_RGBA_0_Vector4.a;
            float2 _Vector2_27b5686b4f2641b1bacfc432832e8b14_Out_0_Vector2 = float2(_SampleTexture2D_f6550e316b954b469bbf81231d66f881_R_4_Float, _SampleTexture2D_f6550e316b954b469bbf81231d66f881_G_5_Float);
            float2 _Add_7f2d0b33a7794677b9d825d9b597f3e8_Out_2_Vector2;
            Unity_Add_float2(_Divide_013b956faa5b45259bd752bda3fe0620_Out_2_Vector2, _Vector2_27b5686b4f2641b1bacfc432832e8b14_Out_0_Vector2, _Add_7f2d0b33a7794677b9d825d9b597f3e8_Out_2_Vector2);
            float2 _Multiply_a69f7deee62e4ea2b796974ed1509798_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Add_7f2d0b33a7794677b9d825d9b597f3e8_Out_2_Vector2, float2(-1, 1), _Multiply_a69f7deee62e4ea2b796974ed1509798_Out_2_Vector2);
            float2 _TilingAndOffset_9b6b6d44caba4ff99f020f7277233356_Out_3_Vector2;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_a69f7deee62e4ea2b796974ed1509798_Out_2_Vector2, _TilingAndOffset_9b6b6d44caba4ff99f020f7277233356_Out_3_Vector2);
            float4 _SampleTexture2D_daf9019a9f404195afb964914c231dea_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_1a4655242b0046a2a32fc421dafca86b_Out_0_Texture2D.tex, _Property_1a4655242b0046a2a32fc421dafca86b_Out_0_Texture2D.samplerstate, _Property_1a4655242b0046a2a32fc421dafca86b_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_9b6b6d44caba4ff99f020f7277233356_Out_3_Vector2) );
            float _SampleTexture2D_daf9019a9f404195afb964914c231dea_R_4_Float = _SampleTexture2D_daf9019a9f404195afb964914c231dea_RGBA_0_Vector4.r;
            float _SampleTexture2D_daf9019a9f404195afb964914c231dea_G_5_Float = _SampleTexture2D_daf9019a9f404195afb964914c231dea_RGBA_0_Vector4.g;
            float _SampleTexture2D_daf9019a9f404195afb964914c231dea_B_6_Float = _SampleTexture2D_daf9019a9f404195afb964914c231dea_RGBA_0_Vector4.b;
            float _SampleTexture2D_daf9019a9f404195afb964914c231dea_A_7_Float = _SampleTexture2D_daf9019a9f404195afb964914c231dea_RGBA_0_Vector4.a;
            float4 _Multiply_4ee8cbd902954acfa842d52747aed6f2_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_daf9019a9f404195afb964914c231dea_RGBA_0_Vector4, (_SampleTexture2D_daf9019a9f404195afb964914c231dea_A_7_Float.xxxx), _Multiply_4ee8cbd902954acfa842d52747aed6f2_Out_2_Vector4);
            UnityTexture2D _Property_5ce238b92e88431cad91d2012f1babdc_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Spot2);
            float2 _Property_b34b4eb1f02040818b8a8b647abd559d_Out_0_Vector2 = _Spot2Coords;
            float2 _Divide_91e86b9383ca4be3820c03268714a476_Out_2_Vector2;
            Unity_Divide_float2(_Property_b34b4eb1f02040818b8a8b647abd559d_Out_0_Vector2, float2(51, 51), _Divide_91e86b9383ca4be3820c03268714a476_Out_2_Vector2);
            UnityTexture2D _Property_22886644eef3469f98d2e1034a092c9b_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_SpotPositions);
            float2 _Vector2_11cb4054545846d39da3453b33f152bd_Out_0_Vector2 = float2(_Divide_152088a4b4f145969cb08e6782e59296_Out_2_Float, float(0.25));
            float4 _SampleTexture2D_5618fb065a324dc99b98fcc0d91af2ba_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_22886644eef3469f98d2e1034a092c9b_Out_0_Texture2D.tex, _Property_22886644eef3469f98d2e1034a092c9b_Out_0_Texture2D.samplerstate, _Property_22886644eef3469f98d2e1034a092c9b_Out_0_Texture2D.GetTransformedUV(_Vector2_11cb4054545846d39da3453b33f152bd_Out_0_Vector2) );
            float _SampleTexture2D_5618fb065a324dc99b98fcc0d91af2ba_R_4_Float = _SampleTexture2D_5618fb065a324dc99b98fcc0d91af2ba_RGBA_0_Vector4.r;
            float _SampleTexture2D_5618fb065a324dc99b98fcc0d91af2ba_G_5_Float = _SampleTexture2D_5618fb065a324dc99b98fcc0d91af2ba_RGBA_0_Vector4.g;
            float _SampleTexture2D_5618fb065a324dc99b98fcc0d91af2ba_B_6_Float = _SampleTexture2D_5618fb065a324dc99b98fcc0d91af2ba_RGBA_0_Vector4.b;
            float _SampleTexture2D_5618fb065a324dc99b98fcc0d91af2ba_A_7_Float = _SampleTexture2D_5618fb065a324dc99b98fcc0d91af2ba_RGBA_0_Vector4.a;
            float2 _Vector2_aaa13365efd4487aba55dec191128c60_Out_0_Vector2 = float2(_SampleTexture2D_5618fb065a324dc99b98fcc0d91af2ba_R_4_Float, _SampleTexture2D_5618fb065a324dc99b98fcc0d91af2ba_G_5_Float);
            float2 _Add_bee702c296294b1d92b760361c7dabc3_Out_2_Vector2;
            Unity_Add_float2(_Divide_91e86b9383ca4be3820c03268714a476_Out_2_Vector2, _Vector2_aaa13365efd4487aba55dec191128c60_Out_0_Vector2, _Add_bee702c296294b1d92b760361c7dabc3_Out_2_Vector2);
            float2 _Multiply_159e2128caff43b0b862185320a8bd23_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Add_bee702c296294b1d92b760361c7dabc3_Out_2_Vector2, float2(-1, 1), _Multiply_159e2128caff43b0b862185320a8bd23_Out_2_Vector2);
            float2 _TilingAndOffset_194b97a9cead48f89f032d23560d4031_Out_3_Vector2;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_159e2128caff43b0b862185320a8bd23_Out_2_Vector2, _TilingAndOffset_194b97a9cead48f89f032d23560d4031_Out_3_Vector2);
            float4 _SampleTexture2D_66b19db5aa244347842e91f753da9fe0_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_5ce238b92e88431cad91d2012f1babdc_Out_0_Texture2D.tex, _Property_5ce238b92e88431cad91d2012f1babdc_Out_0_Texture2D.samplerstate, _Property_5ce238b92e88431cad91d2012f1babdc_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_194b97a9cead48f89f032d23560d4031_Out_3_Vector2) );
            float _SampleTexture2D_66b19db5aa244347842e91f753da9fe0_R_4_Float = _SampleTexture2D_66b19db5aa244347842e91f753da9fe0_RGBA_0_Vector4.r;
            float _SampleTexture2D_66b19db5aa244347842e91f753da9fe0_G_5_Float = _SampleTexture2D_66b19db5aa244347842e91f753da9fe0_RGBA_0_Vector4.g;
            float _SampleTexture2D_66b19db5aa244347842e91f753da9fe0_B_6_Float = _SampleTexture2D_66b19db5aa244347842e91f753da9fe0_RGBA_0_Vector4.b;
            float _SampleTexture2D_66b19db5aa244347842e91f753da9fe0_A_7_Float = _SampleTexture2D_66b19db5aa244347842e91f753da9fe0_RGBA_0_Vector4.a;
            float4 _Multiply_4b5972826eec42a795daaa0fb20666a7_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_66b19db5aa244347842e91f753da9fe0_RGBA_0_Vector4, (_SampleTexture2D_66b19db5aa244347842e91f753da9fe0_A_7_Float.xxxx), _Multiply_4b5972826eec42a795daaa0fb20666a7_Out_2_Vector4);
            float4 _Lerp_1ea7d711e17149d780ba160d4fa84f49_Out_3_Vector4;
            Unity_Lerp_float4(_Multiply_4ee8cbd902954acfa842d52747aed6f2_Out_2_Vector4, _Multiply_4b5972826eec42a795daaa0fb20666a7_Out_2_Vector4, (_SampleTexture2D_66b19db5aa244347842e91f753da9fe0_A_7_Float.xxxx), _Lerp_1ea7d711e17149d780ba160d4fa84f49_Out_3_Vector4);
            UnityTexture2D _Property_ce90526212064425a56a6dea840d20ad_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Spot3);
            float2 _Property_690e21fe71fc437480dedce608823a22_Out_0_Vector2 = _Spot3Coords;
            float2 _Divide_1eabd1c506e841aab79f40fbd0bc4870_Out_2_Vector2;
            Unity_Divide_float2(_Property_690e21fe71fc437480dedce608823a22_Out_0_Vector2, float2(51, 51), _Divide_1eabd1c506e841aab79f40fbd0bc4870_Out_2_Vector2);
            UnityTexture2D _Property_8b30f951ec4b4580bbb508408b626e5a_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_SpotPositions);
            float2 _Vector2_aff52d6a76b94151bc6d45bb7911b253_Out_0_Vector2 = float2(_Divide_152088a4b4f145969cb08e6782e59296_Out_2_Float, float(0.5));
            float4 _SampleTexture2D_7dccf325b07b46099d6877ecdd603bc1_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_8b30f951ec4b4580bbb508408b626e5a_Out_0_Texture2D.tex, _Property_8b30f951ec4b4580bbb508408b626e5a_Out_0_Texture2D.samplerstate, _Property_8b30f951ec4b4580bbb508408b626e5a_Out_0_Texture2D.GetTransformedUV(_Vector2_aff52d6a76b94151bc6d45bb7911b253_Out_0_Vector2) );
            float _SampleTexture2D_7dccf325b07b46099d6877ecdd603bc1_R_4_Float = _SampleTexture2D_7dccf325b07b46099d6877ecdd603bc1_RGBA_0_Vector4.r;
            float _SampleTexture2D_7dccf325b07b46099d6877ecdd603bc1_G_5_Float = _SampleTexture2D_7dccf325b07b46099d6877ecdd603bc1_RGBA_0_Vector4.g;
            float _SampleTexture2D_7dccf325b07b46099d6877ecdd603bc1_B_6_Float = _SampleTexture2D_7dccf325b07b46099d6877ecdd603bc1_RGBA_0_Vector4.b;
            float _SampleTexture2D_7dccf325b07b46099d6877ecdd603bc1_A_7_Float = _SampleTexture2D_7dccf325b07b46099d6877ecdd603bc1_RGBA_0_Vector4.a;
            float2 _Vector2_ef741e573fba4059ae514ea737e7be3e_Out_0_Vector2 = float2(_SampleTexture2D_7dccf325b07b46099d6877ecdd603bc1_R_4_Float, _SampleTexture2D_7dccf325b07b46099d6877ecdd603bc1_G_5_Float);
            float2 _Add_e1f60435d7ec45b4839b736a494ea4df_Out_2_Vector2;
            Unity_Add_float2(_Divide_1eabd1c506e841aab79f40fbd0bc4870_Out_2_Vector2, _Vector2_ef741e573fba4059ae514ea737e7be3e_Out_0_Vector2, _Add_e1f60435d7ec45b4839b736a494ea4df_Out_2_Vector2);
            float2 _Multiply_62f12fdedf2641e593ec81431de9eb86_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Add_e1f60435d7ec45b4839b736a494ea4df_Out_2_Vector2, float2(-1, 1), _Multiply_62f12fdedf2641e593ec81431de9eb86_Out_2_Vector2);
            float2 _TilingAndOffset_ae4d588d9f4944838cfa244aec873af0_Out_3_Vector2;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_62f12fdedf2641e593ec81431de9eb86_Out_2_Vector2, _TilingAndOffset_ae4d588d9f4944838cfa244aec873af0_Out_3_Vector2);
            float4 _SampleTexture2D_0f3cdcfe5f2b43459f3021be6eeef11f_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_ce90526212064425a56a6dea840d20ad_Out_0_Texture2D.tex, _Property_ce90526212064425a56a6dea840d20ad_Out_0_Texture2D.samplerstate, _Property_ce90526212064425a56a6dea840d20ad_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_ae4d588d9f4944838cfa244aec873af0_Out_3_Vector2) );
            float _SampleTexture2D_0f3cdcfe5f2b43459f3021be6eeef11f_R_4_Float = _SampleTexture2D_0f3cdcfe5f2b43459f3021be6eeef11f_RGBA_0_Vector4.r;
            float _SampleTexture2D_0f3cdcfe5f2b43459f3021be6eeef11f_G_5_Float = _SampleTexture2D_0f3cdcfe5f2b43459f3021be6eeef11f_RGBA_0_Vector4.g;
            float _SampleTexture2D_0f3cdcfe5f2b43459f3021be6eeef11f_B_6_Float = _SampleTexture2D_0f3cdcfe5f2b43459f3021be6eeef11f_RGBA_0_Vector4.b;
            float _SampleTexture2D_0f3cdcfe5f2b43459f3021be6eeef11f_A_7_Float = _SampleTexture2D_0f3cdcfe5f2b43459f3021be6eeef11f_RGBA_0_Vector4.a;
            float4 _Multiply_e3e3434263df4d5b9e5c98852fa1cb5a_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_0f3cdcfe5f2b43459f3021be6eeef11f_RGBA_0_Vector4, (_SampleTexture2D_0f3cdcfe5f2b43459f3021be6eeef11f_A_7_Float.xxxx), _Multiply_e3e3434263df4d5b9e5c98852fa1cb5a_Out_2_Vector4);
            float4 _Lerp_866ce6e94f3049ddaeddd9a05e559a3f_Out_3_Vector4;
            Unity_Lerp_float4(_Lerp_1ea7d711e17149d780ba160d4fa84f49_Out_3_Vector4, _Multiply_e3e3434263df4d5b9e5c98852fa1cb5a_Out_2_Vector4, (_SampleTexture2D_0f3cdcfe5f2b43459f3021be6eeef11f_A_7_Float.xxxx), _Lerp_866ce6e94f3049ddaeddd9a05e559a3f_Out_3_Vector4);
            UnityTexture2D _Property_235efdf0225c425a98eb7bf018650791_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Spot4);
            float2 _Property_9b30d54132ab4348ab743ef007b469eb_Out_0_Vector2 = _Spot4Coords;
            float2 _Divide_5b62c88f2abb49f7b7e76927f649c967_Out_2_Vector2;
            Unity_Divide_float2(_Property_9b30d54132ab4348ab743ef007b469eb_Out_0_Vector2, float2(51, 51), _Divide_5b62c88f2abb49f7b7e76927f649c967_Out_2_Vector2);
            UnityTexture2D _Property_a8d7f5f0852d4c98b826ecf75fda08d5_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_SpotPositions);
            float2 _Vector2_ea4b120d92404258933ac842e6c1f028_Out_0_Vector2 = float2(_Divide_152088a4b4f145969cb08e6782e59296_Out_2_Float, float(0.75));
            float4 _SampleTexture2D_0de11f9e4e3346c0acfd9d87d0bf3226_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_a8d7f5f0852d4c98b826ecf75fda08d5_Out_0_Texture2D.tex, _Property_a8d7f5f0852d4c98b826ecf75fda08d5_Out_0_Texture2D.samplerstate, _Property_a8d7f5f0852d4c98b826ecf75fda08d5_Out_0_Texture2D.GetTransformedUV(_Vector2_ea4b120d92404258933ac842e6c1f028_Out_0_Vector2) );
            float _SampleTexture2D_0de11f9e4e3346c0acfd9d87d0bf3226_R_4_Float = _SampleTexture2D_0de11f9e4e3346c0acfd9d87d0bf3226_RGBA_0_Vector4.r;
            float _SampleTexture2D_0de11f9e4e3346c0acfd9d87d0bf3226_G_5_Float = _SampleTexture2D_0de11f9e4e3346c0acfd9d87d0bf3226_RGBA_0_Vector4.g;
            float _SampleTexture2D_0de11f9e4e3346c0acfd9d87d0bf3226_B_6_Float = _SampleTexture2D_0de11f9e4e3346c0acfd9d87d0bf3226_RGBA_0_Vector4.b;
            float _SampleTexture2D_0de11f9e4e3346c0acfd9d87d0bf3226_A_7_Float = _SampleTexture2D_0de11f9e4e3346c0acfd9d87d0bf3226_RGBA_0_Vector4.a;
            float2 _Vector2_e57c57df21584697b1e53df3e8fdef5f_Out_0_Vector2 = float2(_SampleTexture2D_0de11f9e4e3346c0acfd9d87d0bf3226_R_4_Float, _SampleTexture2D_0de11f9e4e3346c0acfd9d87d0bf3226_G_5_Float);
            float2 _Add_9a17f07b62e841d4956243c6db5c8b8c_Out_2_Vector2;
            Unity_Add_float2(_Divide_5b62c88f2abb49f7b7e76927f649c967_Out_2_Vector2, _Vector2_e57c57df21584697b1e53df3e8fdef5f_Out_0_Vector2, _Add_9a17f07b62e841d4956243c6db5c8b8c_Out_2_Vector2);
            float2 _Multiply_e326edeede824a75a7582cbf5ebed0d3_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Add_9a17f07b62e841d4956243c6db5c8b8c_Out_2_Vector2, float2(-1, 1), _Multiply_e326edeede824a75a7582cbf5ebed0d3_Out_2_Vector2);
            float2 _TilingAndOffset_df92dd591b0a478aa5d05850a316a082_Out_3_Vector2;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_e326edeede824a75a7582cbf5ebed0d3_Out_2_Vector2, _TilingAndOffset_df92dd591b0a478aa5d05850a316a082_Out_3_Vector2);
            float4 _SampleTexture2D_2b2329f3587343fb9b1354d0a6e7531c_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_235efdf0225c425a98eb7bf018650791_Out_0_Texture2D.tex, _Property_235efdf0225c425a98eb7bf018650791_Out_0_Texture2D.samplerstate, _Property_235efdf0225c425a98eb7bf018650791_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_df92dd591b0a478aa5d05850a316a082_Out_3_Vector2) );
            float _SampleTexture2D_2b2329f3587343fb9b1354d0a6e7531c_R_4_Float = _SampleTexture2D_2b2329f3587343fb9b1354d0a6e7531c_RGBA_0_Vector4.r;
            float _SampleTexture2D_2b2329f3587343fb9b1354d0a6e7531c_G_5_Float = _SampleTexture2D_2b2329f3587343fb9b1354d0a6e7531c_RGBA_0_Vector4.g;
            float _SampleTexture2D_2b2329f3587343fb9b1354d0a6e7531c_B_6_Float = _SampleTexture2D_2b2329f3587343fb9b1354d0a6e7531c_RGBA_0_Vector4.b;
            float _SampleTexture2D_2b2329f3587343fb9b1354d0a6e7531c_A_7_Float = _SampleTexture2D_2b2329f3587343fb9b1354d0a6e7531c_RGBA_0_Vector4.a;
            float4 _Multiply_956573eb07ff40aebdd30c3b33c53851_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_2b2329f3587343fb9b1354d0a6e7531c_RGBA_0_Vector4, (_SampleTexture2D_2b2329f3587343fb9b1354d0a6e7531c_A_7_Float.xxxx), _Multiply_956573eb07ff40aebdd30c3b33c53851_Out_2_Vector4);
            float4 _Lerp_86cae1c94e4146beb4d6b43ce0b16b6f_Out_3_Vector4;
            Unity_Lerp_float4(_Lerp_866ce6e94f3049ddaeddd9a05e559a3f_Out_3_Vector4, _Multiply_956573eb07ff40aebdd30c3b33c53851_Out_2_Vector4, (_SampleTexture2D_2b2329f3587343fb9b1354d0a6e7531c_A_7_Float.xxxx), _Lerp_86cae1c94e4146beb4d6b43ce0b16b6f_Out_3_Vector4);
            float _Maximum_58dfe2c80609413f93b66794c0dc58a2_Out_2_Float;
            Unity_Maximum_float(_SampleTexture2D_daf9019a9f404195afb964914c231dea_A_7_Float, _SampleTexture2D_66b19db5aa244347842e91f753da9fe0_A_7_Float, _Maximum_58dfe2c80609413f93b66794c0dc58a2_Out_2_Float);
            float _Maximum_19d1234cbbf243519e9851cb3e5ac03d_Out_2_Float;
            Unity_Maximum_float(_Maximum_58dfe2c80609413f93b66794c0dc58a2_Out_2_Float, _SampleTexture2D_0f3cdcfe5f2b43459f3021be6eeef11f_A_7_Float, _Maximum_19d1234cbbf243519e9851cb3e5ac03d_Out_2_Float);
            float _Maximum_2a4c60d3d91045bab963f0bd1234e469_Out_2_Float;
            Unity_Maximum_float(_Maximum_19d1234cbbf243519e9851cb3e5ac03d_Out_2_Float, _SampleTexture2D_2b2329f3587343fb9b1354d0a6e7531c_A_7_Float, _Maximum_2a4c60d3d91045bab963f0bd1234e469_Out_2_Float);
            float4 _Blend_63db57ca102743e0ba2c6fe8e093e384_Out_2_Vector4;
            Unity_Blend_Multiply_float4(_SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_RGBA_0_Vector4, _Lerp_86cae1c94e4146beb4d6b43ce0b16b6f_Out_3_Vector4, _Blend_63db57ca102743e0ba2c6fe8e093e384_Out_2_Vector4, _Maximum_2a4c60d3d91045bab963f0bd1234e469_Out_2_Float);
            float4 _Preview_fe039e510ff94946ac700f7155669a9f_Out_1_Vector4;
            Unity_Preview_float4(_Blend_63db57ca102743e0ba2c6fe8e093e384_Out_2_Vector4, _Preview_fe039e510ff94946ac700f7155669a9f_Out_1_Vector4);
            float4 _Branch_4d8551bfe3f449c2ac8e679a439bd984_Out_3_Vector4;
            Unity_Branch_float4(_Property_d33558c987894df9bf624c76589f84ee_Out_0_Boolean, _Multiply_8199db1c3cb54e7696c6873206648090_Out_2_Vector4, _Preview_fe039e510ff94946ac700f7155669a9f_Out_1_Vector4, _Branch_4d8551bfe3f449c2ac8e679a439bd984_Out_3_Vector4);
            float _Property_ef95125fd9cd422a85117efeb52dcda4_Out_0_Float = _ShadowAlpha;
            float _Branch_b6fb662ef570428baf596a92fa3606fe_Out_3_Float;
            Unity_Branch_float(_Property_d33558c987894df9bf624c76589f84ee_Out_0_Boolean, _Property_ef95125fd9cd422a85117efeb52dcda4_Out_0_Float, float(1), _Branch_b6fb662ef570428baf596a92fa3606fe_Out_3_Float);
            float _Preview_f8a06c9fecd64f90a5b3f436d754997b_Out_1_Float;
            Unity_Preview_float(_SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_A_7_Float, _Preview_f8a06c9fecd64f90a5b3f436d754997b_Out_1_Float);
            float _Multiply_6c102e2410a94b0e935f420646e13837_Out_2_Float;
            Unity_Multiply_float_float(_Branch_b6fb662ef570428baf596a92fa3606fe_Out_3_Float, _Preview_f8a06c9fecd64f90a5b3f436d754997b_Out_1_Float, _Multiply_6c102e2410a94b0e935f420646e13837_Out_2_Float);
            surface.BaseColor = (_Branch_4d8551bfe3f449c2ac8e679a439bd984_Out_3_Vector4.xyz);
            surface.Alpha = _Multiply_6c102e2410a94b0e935f420646e13837_Out_2_Float;
            surface.AlphaClipThreshold = float(0.5);
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    CustomEditorForRenderPipeline "Varguiniano.YAPU.Editor.Shaders.SpindaShaderEditor" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
    FallBack "Hidden/Shader Graph/FallbackError"
}