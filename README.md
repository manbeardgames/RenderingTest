# Rendering Test
This is a test of a rendering workflow for a MonoGame project, however it's not working correctly.  For the sake of brevity, only necessary code is included in the project files. Because of this, some things may seem odd, such as "Why does the `Renderer` class just have a single `Texture2D` that it renders?".  In the final version, each renderer will be smart enough to know what to render, it was just set this way in this repo for the test.

# The Goal Of This Workflow
The goal of this is to 
1. Provide resolution independent rendering by creating letterbox/pillerbox when needed
2. Provide a render pipeline where (in order)
    1. Each **Renderer** renders to its own target at virtual resolution.
    2. Each of these targets are rendered to a final target at virtual resolution.
    3. The final target is rendered to screen scaled to create letterbox/pillerbox when needed.

# What Isn't Working.
Currently, when rendering is done at 1:1 between the base resolution and virtual resolution, everything
look proper. However, when cutting the base resolution in half, only the top-left quadrant of the virtual resolution seems to be rendered.  See the images below to go with this.

**Only top left quadrant is rendered when half base resolution**  
![](https://raw.githubusercontent.com/manbeardgames/RenderingTest/master/half_resolution_render.png)

**Renders correctly when base and virtual resolution match**  
![](https://raw.githubusercontent.com/manbeardgames/RenderingTest/master/full_resolution_render.png)



