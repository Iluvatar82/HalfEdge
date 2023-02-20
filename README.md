<a name="readme-top"></a>

<!-- PROJECT LOGO -->
<br />
<div align="center">
  <a href="https://github.com/Iluvatar82/HalfEdge">
    <img src="/Assets/Logo.svg" alt="Logo" width="80" height="80">
  </a>

  <h3 align="center">HalfEdge</h3>

  <p align="center">
    A HalfEdge Data-Structure to use in your projects!
    <br />
    <br />
    <a href="https://github.com/Iluvatar82/HalfEdge/issues">Report Bug</a>
    ·
    <a href="https://github.com/Iluvatar82/HalfEdge/issues">Request Feature</a>
  </p>
</div>

<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

This Project aims to provide you with an easy-to-use (I am working on that one :)) Library to use in your Projects. The Feature-Set is kind of restricted at the moment, but that will change in the Future.

Benefits of using this Library:
* You can use this in a wide range of applications - to work with a mesh.
* I did it, so you won't have to write your own Library which does the same thing
* I try to keep the Code clean so everything can be used easily
* New features regarding the Mesh will follow soon:
  * Subdivision of the Mesh
  * Cutting of the Mesh with regards to a Plane (Axis-Aligned at the beginning, but general Planes will be supported in the Future)
  * Boolean Operations with multiple Meshes, like the Union and Difference

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- GETTING STARTED -->
## Getting Started

After including the Library in your Project, either via adding the Code directly to your solution or by adding a Reference to the DLL-File. You can start using the Library to do your computations.
Simple Conversions exist e.g. for the <a href="/Models/Base/Vertex.cs">Vertex</a> class to support an easy way to create your first <a href="/Models/Base/Mesh.cs">Mesh</a>.

### Installation

Download the Source-Code and add the Project to your Solution or get on the the generated DLLs and use this in your Project.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- USAGE EXAMPLES -->
## Usage

To get a valid <a href="/Models/Base/Mesh.cs">Mesh</a>, the most convenient way is to use the <a href="/HalfEdge/MeshFactory.cs">MeshFactory</a> (which is a static class) and let it generate the Half-Edge Information for you! You just provide all Vertices and the Polygon-Indices in the Vertex-List.
A simple example would be:
```cs
var vertices = new List<Vertex> { new Vertex(0, 0, 0), new Vertex(2, 1, 1), new Vertex(0, 1, 1) };
var indices = new List<List<int>> { new List<int> { 0, 1, 2 } };
var mesh = MeshFactory.CreateMesh(vertices, indices);
```
Of course the Mesh is not restricted to only consist of triangles (this is where the <a href="/Models/TriangleMesh.cs">TriangleMesh</a> comes into play), so you can create Meshes with e.g. only Polygons consisting of five and six Vertices.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- ROADMAP -->
## Roadmap

- [x] Add back to top links here
- [ ] Add Changelog
- [ ] More convenient Way of adding/removing Polygons (without the need to add the Vertices and HalfEdges beforehand)
- [ ] HalfEdge Subdivision(s) (probably multiple Variants)
- [ ] Adding Texture-Coordinates (maybe?)
- [ ] Mesh-Cutoffs
- [ ] Boolean Operations for two Meshes
    - [ ] Union
    - [ ] Difference

See the <a href="https://github.com/Iluvatar82/HalfEdge/issues">open Issues</a> for a full list of proposed features (and known issues).

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- CONTACT -->
## Contact

Project Link: [https://github.com/Iluvatar82/HalfEdge](https://github.com/Iluvatar82/HalfEdge)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- ACKNOWLEDGMENTS -->
## Acknowledgments

Use this space to list resources you find helpful and would like to give credit to. I've included a few of my favorites to kick things off!

* [Choose an Open Source License](https://choosealicense.com)
* [Sweep Line Polygon Triangulation](https://www.cs.ucsb.edu/~suri/cs235/Triangulation.pdf)

<p align="right">(<a href="#readme-top">back to top</a>)</p>
