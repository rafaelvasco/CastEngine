$input v_color0, v_texcoord0

#include "common.sh"

SAMPLER2D(texture_2d, 0);

void main()
{
	gl_FragColor = texture2D(texture_2d, v_texcoord0)  * v_color0;
}