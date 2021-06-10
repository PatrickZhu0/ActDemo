// Unlit shader. Simplest possible textured shader.
// - DFM Simplest Shader

Shader "DFM/NormalMonster" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}

SubShader {
	Tags { "RenderType"="Opaque" "Queue"="Transparent-10"}
	LOD 100
	
	// Non-lightmapped
	Pass {
        Name "NORMALMONSTER"
		Tags { "LightMode" = "Vertex" }
		Lighting Off
		SetTexture [_MainTex] { combine texture } 
	}
}
}



