Shader "YAPU/AnimatedFlipbookSpriteCompiled"
{
    Properties
    {
        [NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
        _SpriteSheet("SpriteSheet", 2D) = "white" {}
        _Flipbook("Flipbook", Vector) = (1, 1, 0, 0)
        _Speed("Speed", Float) = 10
        [ToggleUI]_SpecificIndex("SpecificIndex", Float) = 0
        _MinIndex("MinIndex", Float) = 0
        _MaxIndex("MaxIndex", Float) = 15
        [ToggleUI]_IsShadow("IsShadow", Float) = 0
        _ShadowColor("ShadowColor", Color) = (0.2264151, 0.2264151, 0.2264151, 0)
        _ShadowAlpha("ShadowAlpha", Range(0, 1)) = 0.6
        [ToggleUI]_AutoAnimate("AutoAnimate", Float) = 1
        _SpriteIndex("SpriteIndex", Int) = 0
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
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SpriteSheet);
        SAMPLER(sampler_SpriteSheet);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
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
        
        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
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
            float4 _Multiply_516837854e9a4476be2e16ca00e369c6_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_RGBA_0_Vector4, (_SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_A_7_Float.xxxx), _Multiply_516837854e9a4476be2e16ca00e369c6_Out_2_Vector4);
            float4 _Branch_4d8551bfe3f449c2ac8e679a439bd984_Out_3_Vector4;
            Unity_Branch_float4(_Property_d33558c987894df9bf624c76589f84ee_Out_0_Boolean, _Multiply_8199db1c3cb54e7696c6873206648090_Out_2_Vector4, _Multiply_516837854e9a4476be2e16ca00e369c6_Out_2_Vector4, _Branch_4d8551bfe3f449c2ac8e679a439bd984_Out_3_Vector4);
            float _Property_ef95125fd9cd422a85117efeb52dcda4_Out_0_Float = _ShadowAlpha;
            float _Branch_b6fb662ef570428baf596a92fa3606fe_Out_3_Float;
            Unity_Branch_float(_Property_d33558c987894df9bf624c76589f84ee_Out_0_Boolean, _Property_ef95125fd9cd422a85117efeb52dcda4_Out_0_Float, float(1), _Branch_b6fb662ef570428baf596a92fa3606fe_Out_3_Float);
            float _Multiply_6c102e2410a94b0e935f420646e13837_Out_2_Float;
            Unity_Multiply_float_float(_Branch_b6fb662ef570428baf596a92fa3606fe_Out_3_Float, _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_A_7_Float, _Multiply_6c102e2410a94b0e935f420646e13837_Out_2_Float);
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
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SpriteSheet);
        SAMPLER(sampler_SpriteSheet);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
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
            float _Multiply_6c102e2410a94b0e935f420646e13837_Out_2_Float;
            Unity_Multiply_float_float(_Branch_b6fb662ef570428baf596a92fa3606fe_Out_3_Float, _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_A_7_Float, _Multiply_6c102e2410a94b0e935f420646e13837_Out_2_Float);
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
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SpriteSheet);
        SAMPLER(sampler_SpriteSheet);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
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
            float _Multiply_6c102e2410a94b0e935f420646e13837_Out_2_Float;
            Unity_Multiply_float_float(_Branch_b6fb662ef570428baf596a92fa3606fe_Out_3_Float, _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_A_7_Float, _Multiply_6c102e2410a94b0e935f420646e13837_Out_2_Float);
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
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SpriteSheet);
        SAMPLER(sampler_SpriteSheet);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
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
        
        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
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
            float4 _Multiply_516837854e9a4476be2e16ca00e369c6_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_RGBA_0_Vector4, (_SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_A_7_Float.xxxx), _Multiply_516837854e9a4476be2e16ca00e369c6_Out_2_Vector4);
            float4 _Branch_4d8551bfe3f449c2ac8e679a439bd984_Out_3_Vector4;
            Unity_Branch_float4(_Property_d33558c987894df9bf624c76589f84ee_Out_0_Boolean, _Multiply_8199db1c3cb54e7696c6873206648090_Out_2_Vector4, _Multiply_516837854e9a4476be2e16ca00e369c6_Out_2_Vector4, _Branch_4d8551bfe3f449c2ac8e679a439bd984_Out_3_Vector4);
            float _Property_ef95125fd9cd422a85117efeb52dcda4_Out_0_Float = _ShadowAlpha;
            float _Branch_b6fb662ef570428baf596a92fa3606fe_Out_3_Float;
            Unity_Branch_float(_Property_d33558c987894df9bf624c76589f84ee_Out_0_Boolean, _Property_ef95125fd9cd422a85117efeb52dcda4_Out_0_Float, float(1), _Branch_b6fb662ef570428baf596a92fa3606fe_Out_3_Float);
            float _Multiply_6c102e2410a94b0e935f420646e13837_Out_2_Float;
            Unity_Multiply_float_float(_Branch_b6fb662ef570428baf596a92fa3606fe_Out_3_Float, _SampleTexture2D_7a46cf0e97444b3fb6b914b87a7e2c2f_A_7_Float, _Multiply_6c102e2410a94b0e935f420646e13837_Out_2_Float);
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
    CustomEditorForRenderPipeline "Varguiniano.YAPU.Editor.Shaders.AnimatedFlipbookSpriteEditor" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
    FallBack "Hidden/Shader Graph/FallbackError"
}