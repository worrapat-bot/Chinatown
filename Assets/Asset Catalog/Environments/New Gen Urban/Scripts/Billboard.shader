// Made with Amplify Shader Editor v1.9.3.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Billboard"
{
	Properties
	{
		_MainTex("Texture Sample 0", 2D) = "white" {}
		_PixelDensity("Pixel Density", Float) = 0
		_Float11("Float 11", Float) = 0
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
		_Emission("Emission", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _Float11;
		uniform sampler2D _MainTex;
		uniform float _PixelDensity;
		uniform float _Emission;
		uniform sampler2D _TextureSample1;
		uniform float4 _TextureSample1_ST;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 color6 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
			float4 color7 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
			float2 temp_cast_0 = (_Float11).xx;
			float2 FinalUV13_g1 = ( temp_cast_0 * ( 0.5 + i.uv_texcoord ) );
			float2 temp_cast_1 = (0.5).xx;
			float2 temp_cast_2 = (1.0).xx;
			float4 appendResult16_g1 = (float4(ddx( FinalUV13_g1 ) , ddy( FinalUV13_g1 )));
			float4 UVDerivatives17_g1 = appendResult16_g1;
			float4 break28_g1 = UVDerivatives17_g1;
			float2 appendResult19_g1 = (float2(break28_g1.x , break28_g1.z));
			float2 appendResult20_g1 = (float2(break28_g1.x , break28_g1.z));
			float dotResult24_g1 = dot( appendResult19_g1 , appendResult20_g1 );
			float2 appendResult21_g1 = (float2(break28_g1.y , break28_g1.w));
			float2 appendResult22_g1 = (float2(break28_g1.y , break28_g1.w));
			float dotResult23_g1 = dot( appendResult21_g1 , appendResult22_g1 );
			float2 appendResult25_g1 = (float2(dotResult24_g1 , dotResult23_g1));
			float2 derivativesLength29_g1 = sqrt( appendResult25_g1 );
			float2 temp_cast_3 = (-1.0).xx;
			float2 temp_cast_4 = (1.0).xx;
			float2 clampResult57_g1 = clamp( ( ( ( abs( ( frac( ( FinalUV13_g1 + 0.25 ) ) - temp_cast_1 ) ) * 4.0 ) - temp_cast_2 ) * ( 0.35 / derivativesLength29_g1 ) ) , temp_cast_3 , temp_cast_4 );
			float2 break71_g1 = clampResult57_g1;
			float2 break55_g1 = derivativesLength29_g1;
			float4 lerpResult73_g1 = lerp( color6 , color7 , saturate( ( 0.5 + ( 0.5 * break71_g1.x * break71_g1.y * sqrt( saturate( ( 1.1 - max( break55_g1.x , break55_g1.y ) ) ) ) ) ) ));
			float2 temp_cast_5 = (( _Time.y * 0.2 )).xx;
			float2 uv_TexCoord19 = i.uv_texcoord * float2( 0,20 ) + temp_cast_5;
			float simplePerlin2D21 = snoise( uv_TexCoord19 );
			simplePerlin2D21 = simplePerlin2D21*0.5 + 0.5;
			float2 temp_cast_6 = (( 1.0 - round( ( simplePerlin2D21 * 2.0 ) ) )).xx;
			float2 blendOpSrc33 = temp_cast_6;
			float2 blendOpDest33 = i.uv_texcoord;
			float2 lerpBlendMode33 = lerp(blendOpDest33,abs( blendOpSrc33 - blendOpDest33 ),0.005);
			float4 blendOpSrc3 = lerpResult73_g1;
			float4 blendOpDest3 = tex2D( _MainTex, ( saturate( lerpBlendMode33 )) );
			float4 lerpBlendMode3 = lerp(blendOpDest3,(( blendOpDest3 > 0.5 ) ? ( 1.0 - 2.0 * ( 1.0 - blendOpDest3 ) * ( 1.0 - blendOpSrc3 ) ) : ( 2.0 * blendOpDest3 * blendOpSrc3 ) ),_PixelDensity);
			float4 temp_output_3_0 = ( saturate( lerpBlendMode3 ));
			o.Albedo = temp_output_3_0.rgb;
			o.Emission = ( temp_output_3_0 * _Emission ).rgb;
			float2 uv_TextureSample1 = i.uv_texcoord * _TextureSample1_ST.xy + _TextureSample1_ST.zw;
			o.Smoothness = tex2D( _TextureSample1, uv_TextureSample1 ).r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19302
Node;AmplifyShaderEditor.SimpleTimeNode;24;-2606.196,-600.2067;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;26;-2573.892,-305.7066;Inherit;False;Constant;_Float12;Float 12;4;0;Create;True;0;0;0;False;0;False;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;20;-2163.634,-513.4749;Inherit;False;Constant;_Vector1;Vector 1;4;0;Create;True;0;0;0;False;0;False;0,20;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-2372.491,-438.7071;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;19;-1944.609,-542.1637;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;21;-1652.144,-527.9701;Inherit;False;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-1390.037,-421.0537;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RoundOpNode;23;-1172.353,-506.4553;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;31;-990.1933,-405.1656;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;32;-976.6899,-625.3742;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;5;-750.0869,295.2348;Inherit;False;Property;_Float11;Float 11;2;0;Create;True;0;0;0;True;0;False;0;207.54;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;6;-911.2902,9.234899;Inherit;False;Constant;_Color2;Color 2;3;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;7;-963.2919,206.8351;Inherit;False;Constant;_Color3;Color 3;3;0;Create;True;0;0;0;False;0;False;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BlendOpsNode;33;-706.6899,-464.3742;Inherit;False;Difference;True;3;0;FLOAT;0;False;1;FLOAT2;0,0;False;2;FLOAT;0.005;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-526.4826,345.9347;Inherit;False;Property;_PixelDensity;Pixel Density;1;0;Create;True;0;0;0;True;0;False;0;0.15;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1;-544.0762,144.1716;Inherit;False;Checkerboard;-1;;1;43dad715d66e03a4c8ad5f9564018081;0;4;1;FLOAT2;0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;FLOAT2;0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;2;-635.2964,-88.7562;Inherit;True;Property;_MainTex;Texture Sample 0;0;0;Create;False;0;0;0;True;0;False;2;None;536dac0521190474cbc8a0973f0e9de8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BlendOpsNode;3;-233.9003,-0.2124023;Inherit;False;Overlay;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-216.0443,263.5834;Inherit;False;Property;_Emission;Emission;4;0;Create;True;0;0;0;True;0;False;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;8;-521.0336,570.4837;Inherit;True;Property;_TextureSample1;Texture Sample 1;3;0;Create;True;0;0;0;True;0;False;-1;None;0348a46a533bbdf4cacc764f421e99a8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;2.845581,146.5743;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;178.0858,-0.3056146;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Billboard;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;25;0;24;0
WireConnection;25;1;26;0
WireConnection;19;0;20;0
WireConnection;19;1;25;0
WireConnection;21;0;19;0
WireConnection;27;0;21;0
WireConnection;23;0;27;0
WireConnection;31;0;23;0
WireConnection;33;0;31;0
WireConnection;33;1;32;0
WireConnection;1;2;6;0
WireConnection;1;3;7;0
WireConnection;1;4;5;0
WireConnection;2;1;33;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;3;2;4;0
WireConnection;34;0;3;0
WireConnection;34;1;35;0
WireConnection;0;0;3;0
WireConnection;0;2;34;0
WireConnection;0;4;8;0
ASEEND*/
//CHKSM=98CF8D952F57D33356684A5EF6D93A637E6E2876