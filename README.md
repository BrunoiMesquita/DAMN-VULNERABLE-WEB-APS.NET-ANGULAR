<h3>What is DAMN Stack</h3>

<p>DAMN = Docker. Angular. MongoDB. and .NET.</p>
<p>
  DAMN Stack is an open source project, designed to get new projects up and going quickly, by providing a template and pattern to follow. It is created with
  <a href="http://angular.io" target="_blank">Angular</a> (for the UI),
  <a href="https://www.microsoft.com/net/core#windowscmd" target="_blank">.NET Core</a> (for the API), and
  <a href="https://www.mongodb.com/" target="_blank">MongoDB</a> (...for the DB).
</p>

<h3>Get Started</h3>
<p>Use DAMN Stack with your next project.</p>
<ol>
  <li>Clone the stack</li>
  <li>Rename the API solution and project</li>
  <li>If desired, change the folders and namespaces to match your naming schemas.</li>
</ol>


<p>Project Source Repo:</p>
<p><a href="https://github.com/dahln/damn" target="_blank">https://github.com/dahln/damn</a></p>

<p>Project Deployment Repo:</p>
<p><a href="https://github.com/dahln/damn-deploy" target="_blank">https://github.com/dahln/damn-deploy</a></p>

<p>Docker Hub Repo:</p>
<p><a href="https://hub.docker.com/r/dahln/damn-deploy/" target="_blank">https://hub.docker.com/r/dahln/damn-deploy/</a></p>

<p>Contact/FAQ/Support:</p>
<p><a href="mailto:damnstack@outlook.com">damnstack@outlook.com</a></p>

<h3>History of DAMN Stack</h3>
<p>
  DAMN Stack has taken several forms over that past few years. It was originally a project template on the Visual Studio Gallery (Under a different name using .NET, AngularJS, and SQL). That project eventually evolved into the Glubfish Stack. Damn Stack is the next iteration of this application stack. The transition to DAMN Stack adds a new tool that allows developers to specify a projet name and generate a new project based on the DAMN Stack but with the project name specified.
</p>

<h3>About This Project</h3>
<p>
  The <strong>demo</strong> for this stack can be found here: <a href="https://damnstack.org">https://damnstack.org</a>
</p>
<p>
  <strong>Note: Because this site is for demonstration, the database could be wiped/recreated at any time.</strong>
</p>

<p>This demo application is a simple Todo list, but it provides a solid starting point for more advanced applications. The basic requirements for the project are as follows:</p>
<ul>
  <li>Angular Frontend App</li>
  <li>.NET Core API</li>
  <li>MongoDB</li>
  <li>Allow a user to register for an account</li>
  <li>Require a user to authenticate with a valid username and password to sign in</li>
  <li>Allow the user to manage a Todo list</li>
  <li>Allow the user to manage their profile</li>
  <li>Angular - have a clean startup process (include determining if the user is already signed in)</li>
  <li>Angular - maintain session across browser/tab closing</li>
  <li>Angular - Use Bootstrap JS (latest production release)</li>
  <li>Angular - Use Typescript</li>
  <li>Angular - Create a library of external services to consume. Inlude the library in any components that need to use a web service.</li>
  <li>.NET Core - expose functionality via an API</li>
  <li>.NET Core - enforce authorization for all web necessary services</li>
  <li>.NET Core - Use JWT tokens for security. Store the token in a cookie.</li>
  <li>.NET Core - Use dependecy injection</li>
  <li>.NET Core - Use MongoDB as the data store</li>
  <li>MongoDB - Store Data</li>
</ul>

<h3>Deployment</h3>
<p>There are three ways to run this application.</p>
<p>You can run the App and DB manually. Ensure your connection strings match. This method is good for development/debugging.</p>
<p>The second way you can run the application using the docker-compose in the root folder of the project. Run 'docker-compose build' to build, then 'docker-compose up' to run the application. This method is good to test the docker images.</p>
<p>The third way you can run this application is to pull the images from docker hub. No code - just run in the docker image. To do this, visit this repo <a href="https://github.com/damnstack/stack-deploy">stack-deploy</a> at github. This repo contains a docker-compose file which points to the published images in the public repo for this project at docker hub. This method is great for production. On your production machine, 'git clone https://github.com/damnstack/stack-deploy.git'. After cloned, run 'docker-compose build' and then 'docker-compose up' (add the -d flag to run the application in the background)</p>
<p>The main "code" project has a file, dockerTagPushToHub.bat. This will push the latest project image to the connected repo. If you use this file, change the image version numbers in there and make them match the version numbers in the docker-compose in the glubfish-deploy project. After you pushing the latest images to docker hub repo, pull the latest docker-compose to your prodcution machine. If you updated the docker-compose to have the latest image version numbers, when should be able to deploye easily.</p>
<h4>Deployment Notes</h4>
<p>NOTE: The docker compose file specifies a storage volume for the mongodb database. That path is for a linux box. Change it for your particular configuration</p>
<p>NOTE: This image is setup with an SSL certificate, associated with the damnstack.org domain. You can run these images, but you will get a certificate warning. Visit the github for the project source, and modify the images to include your SSL cert.</p>

<h3>Maintenance</h3>
<p>December 2018</p>
<ul>
  <li>CORS Fixes for .NET Core 2.2</li>
  <li>Separated the .NET Core and Angular applications into Client/Server applications.</li>
  <li>Updated dockerfiles to match new structure</li>
  <li>Moved to new github repo github.com/dahln/damn</li>
  <li>Moved to new docker repo hub.docker.com/r/dahln/damn-deploy</li>
</ul>


<p>December 2018</p>
<ul>
  <li>Updated do .NET Core 2.2</li>
  <li>Update to Angular 7.1.2</li>
  <li>Changed host domain from damnstack.com to damnstack.org</li>
</ul>

<p>November 2018</p>
<ul>
  <li><strong>Major Changes</strong></li>
  <li>Wrapped the angular application in a .NET Core wrapper.</li>
  <li>Angular and API no run as a single application. Api can be comsumed without the fronted.</li>
  <li>CORS support for API is removed, but can easily be added back in if necessary</li>
  <li>New Dockerfiles and docker-compose files for new application architecture</li>
  <li>Separated solution into multiple projects: App, Model, Logic, and Data</li>
  <li>Updated the latest .NET Core version</li>
  <li>Updated Angular version</li>
  <li>Switching docker image version away from timebased version number</li>
</ul>

<p>September 2018</p>
<ul>
  <li>Glubfish Stack rebranded as Damn Stack</li>
  <li>New damnstack.com site</li>
  <li>Update demo UI</li>
  <li>Updated to .NET Core 2.1.3</li>
</ul>

<p>August 2018</p>
<ul>
  <li>Updated Angular to 6.1.1</li>
  <li>Updated to .NET Core 2.1.2</li>
</ul>

<p>July 2018</p>
<ul>
  <li>Updated Angular to 6.0.7</li>
  <li>Updated to .NET Core 2.1.1</li>
  <li>Updated .NET Core Dockerfile to work with .NET Core 2.1.1</li>
</ul>

<p>May 2018</p>
<ul>
  <li>Updated Angular to 6.0.0</li>
  <li>Switched to Bootstrap CDN</li>
  <li>Switched to FontAwesome 5 and CDN</li>
  <li>Switched to BootBox CDN</li>
  <li>Added Dockerfile's and Docker Compose</li>
  <li>Moved project from Visual Studio Gallery to Github</li>
</ul>

<p>February 2018</p>
<ul>
  <li>General code clean up for the API</li>
  <li>Updated Angular to 5.2.4</li>
  <li>Updated to Bootstrap 4</li>
  <li>Added FontAwesome 4 (Bootstrap dropped Glyphicon support)</li>
  <li>Support for FontAwesome 5 will come when FontAwesome releases their Angular library</li>
  <li>
    Replaced ngx-modialog with
    <a href="http://bootboxjs.com/" target="_blank">BootBox.js</a>
  </li>
</ul>

<p>December 2017</p>
<ul>
  <li>Initial development of new project</li>
  <li>Replaced old AngularJS project</li>
  <li>Replaced old .NET 4.6 project</li>
  <li>Switched to MongoDB from SQL</li>
</ul>

