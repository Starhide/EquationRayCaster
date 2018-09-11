// Camera
float near;
float3 campos;
float3 forward;
float3 left;
float3 up;

int width;
int height;
static int interations = 12;

// Texture
texture2D ScreenTexture;

sampler TextureSampler = sampler_state
{
	Texture = <ScreenTexture>;
};

float3 ScreenToWorld(float2 screen, float2 pos) {
	float x = ((pos.x - screen.x / 2) / (screen.x / 2)) / 2;
	float y = ((pos.y - screen.y / 2) / (screen.y / 2)) / 2;

	float3 worldpos = campos + forward * near + left * x + up * (-y);
	
	return worldpos;
}
// Equation

float4 PixelShaderFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
	float4 color = float4(0,0,0,1);

	float3 world = ScreenToWorld(float2(width, height), texCoord);
	world = normalize(world);

	bool done = false;

	float3 start = campos;
	float3 curr = campos;
	float increment = 1;
	float goal = 1;
	float o = 0;
	float po = -1;

	//[unroll(20)]while (!done) {
		for (int k = 0; k < interations; k++)
		{
			curr = start + world * increment * k;

			o = 7 * curr.x * curr.y / exp(curr.x * curr.x + curr.y * curr.y) - curr.z;

			if (abs(goal - o) < 0.01)
			{
				//output[index] = Vector3.Distance(cam.Position, pos);
				float xc = abs(curr.x);
				float yc = abs(curr.y);
				float zc = abs(curr.z);
				float s = xc + yc + zc;
				color = float4(zc / s, yc / s, xc / s, 1);
				done = true;
				break;
			}
			else
			{
				if ((o < goal && po > goal) || (o > goal && po < goal))
				{
					start = start + world * increment * (k - 1);
					increment = increment / interations;
					break;
				}
			}

			po = o;

			if (k == interations - 1)
			{
				color = float4(0,0,0,1);
				done = true;
			}
		//}
	}

	return color;
}

technique BlackAndWhite
{
	pass Pass1
	{
		PixelShader = compile ps_4_0_level_9_3  PixelShaderFunction();
	}
}