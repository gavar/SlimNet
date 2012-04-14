/*
 * SlimNet - Networking Middleware For Games
 * Copyright (C) 2011-2012 Fredrik Holmström
 * 
 * This notice may not be removed or altered.
 * 
 * This software is provided 'as-is', without any expressed or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software. 
 * 
 * Attribution
 * The origin of this software must not be misrepresented; you must not
 * claim that you wrote the original software. For any works using this 
 * software, reasonable acknowledgment is required.
 * 
 * Noncommercial
 * You may not use this software for commercial purposes.
 * 
 * Distribution
 * You are not allowed to distribute or make publicly available the software 
 * itself or its source code in original or modified form.
 */

Shader "SlimNet/Name Shader" { 
	Properties { 
	   _MainTex ("Font Texture", 2D) = "white" {} 
	   _Color ("Text Color", Color) = (1,1,1,1) 
	}

	SubShader { 
	   Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" } 
	   Lighting Off Cull Off ZWrite Off Fog { Mode Off } 
	   Blend SrcAlpha OneMinusSrcAlpha 
	   Pass { 
		  Color [_Color] 
		  SetTexture [_MainTex] { 
			 combine primary, texture * primary 
		  }
	   } 
	} 
}