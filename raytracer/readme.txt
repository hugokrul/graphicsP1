Hugo Krul: 8681929
Koen Vermeulen: 4729382

A statement about what minimum requirements and bonus assignments you have implemented (if any) 
and information that is needed to grade them, including detailed information on your implementation.
[We will not search for features in your code. If we can’t find and understand them easily, 
they maynot be graded, so make sure your description and/or comments are clear.]

Minimum requirements
Camera:
- Support FOV:
	We calculated the fov from the degrees using tan(degrees/2). This gives us a distance from the camera to the
	plane which we use to calculate further.

- Arbitrary position and orientation:
	We use WASD, Shift and Space to move the keyboard arrows to rotate and QE to roll.
	For every frame, we update the position of the screen in corralation with the camera position which we calculate.

Primitives: 
- Support of planes and spheres
	You can add Planes and Spheres classes to the scene. The scene is a List<Primitive> which is used by the raytracer
	to loop around every primitive and calculate if a ray was intersected with the primitive.

Lights

 - Arbitrary number of lights
	The raytracer correctly handles an arbitrary number of point lights and calculates the shadows
	that those lights generate accordingly. The shadows add up if they overlap.
	Method (in code):
		- Go through every light in the scene
		- Debug the shadow rays, depending on the shape. Planes are excluded as the ground generates too many rays.
		- If the shadow ray doesn't hit anything calculate the pixel color
		- Ahoot a shadow ray from the intersection to the light
		- Check if the ray hits anything (with a margin of 0.0001 to eliminate light acne)
		- If there is no shadow in this pixel calculate the Lradiance of the pixel compared to the current light, get the Reflected color and the normal of the current primitive type and add the PixelColor which is calculated with the diffusement and glossiness added together

Materials:
 - Phong shading model 
	implemented by multiplying the Light radiance with the sum of the diffusing and the glossiness of the primitive

 - Pure specular
	After an intersection with a pure specular primitive, the primitive fires a new ray (with an adjustable cap)
	from the intersection to the reflection direction and returns the color of the new intersection made by this ray.

- Ground flour texturing
	The ground floor is textured with the checkerboard pattern and overlayed with the base color of the plane
	(it still accepts shadows and glossiness)



Demonstration Scene:
- include a demonstration scene
	Our raytracer has two functions: Render() and RenderDebug(), RenderDebug() shows the debug output on the left side of the screen
	which gives a overhead view of the scene so you can see where the camera is in relation with each primitive.
	
	Render() goes through every pixel, makes a ray with those x and y coördinates and
	intersects that ray with each primitive in the scene. It calculates the closest primitive and gives corrisponding
	pixel the color of that primitive.

Application:
- Handle Keyboard inputs:
	We choose to work with only keyboard inputs.

	W: move forward in relation to the X and Z direction you are looking at.
	S: move backwards in relation to the X and Z direction you are looking at.
	A: move left in relation to the X and Z direction you are looking at.
	D: move right in relation to the X and Z direction you are looking at.

	Spacebar: move up.
	LeftShift or RightShift: move down.

	LeftArrow: look to the left.
	RightArrow: look to the right.
	UpArrow: look up.
	DownArrow: look down.

	Q: roll to the left.
	E: roll to the right.

	T: reset the camera to position (0,0,0) with direction (0,0,1).

Debug output:
- Primitives
	The debug output ones again loops through all the primitives and shows basic lines on where to find them.
	It also shows where the lights are located and where the camera and the camera screen is.

	It also calculates primary rays with certain X and Z values and Y = screen.height/2. 
	These rays are ones again intersected with the scene, but only to calculate the distance the rays have to be.
	These rays are yellow in color.

	The secondary rays show rays from the mirror to the closest intersection point. It doesn't show that they intersect with the plane.
	There would be too many rays if we would do that. The secondary rays are white in color.

	The Shadow rays are from the light source to the closest intersection point. We choose to stop at the intersection point,
	because there are too many rays if we don't stop there.
	The rays are gray in color.



Bonus assignments
Triangle primitive:
	We implemented a triangle primitive which has 3 corners, and a pyramide primitve which has 4 corners.
	The pyramid primitive adds triangles to the scene that make up a pyramide.
	The intersection is calculated by making all the triangles a plane, then intersecting a ray with that plane
	and calculating if that point is inside the triangle or not.

Spotlights:
	We calculate if the anlge between the lightray and the direction is bigger than the maximum degrees
	which we provided in the spotlight constructor. If that's the case the ray gets deleted, if not the ray gets put through the raytracer.




Materials used:
	- The slides
	- https://samsymons.com/blog/math-notes-ray-plane-intersection/ for the intersection with the planes
	- Our amazing minds