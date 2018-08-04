<h2>What is Glubfish</h2>

<p>Glubfish Stack is an open source project, designed to help a project get started faster. It is created with
  <a href="http://angular.io" target="_blank">Angular</a> (for the UI),
  <a href="https://www.microsoft.com/net/core#windowscmd" target="_blank">.NET Core</a> (for the API), and
  <a href="https://www.mongodb.com/" target="_blank">MongoDB</a> (...for the DB).
</p>
<p>The goal of the project is to provide a starting point for new software applications, using a combination of tried-and-true technologies, brought together into a well architected, scalable system.</p>

<p>Project Source Repo:</p>
<p><a href="https://github.com/dahln/glubfish">https://github.com/dahln/glubfish</a></p>

<p>Project Deployment Repo:</p>
<p><a href="https://github.com/dahln/glubfish-deploy.git">https://github.com/dahln/glubfish-deploy.git</a></p>

<p>Docker Hub Repo:</p>
<p><a href="https://hub.docker.com/r/dahln/glubfish/">https://hub.docker.com/r/dahln/glubfish/</a></p>

<p>Contact/FAQ/Support:</p>
<p><a href="mailto:glubfish@outlook.com">glubfish@outlook.com</a></p>

<h3>About This Project</h3>
<p>
    The demo for this stack can be found here: <a href="https://glubfish.dahln.net">https://glubfish.dahln.net</a>
</p>
<p>
  <strong>Because this site is for demonstration, the database could be wiped/recreated at any time.</strong>
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
<p>You can run the API, the UI, and DB manually. Ensure your connection strings match. This method is good for development/debugging.</p>
<p>The second way you can run the application using the docker-compose in the root folder of the project. Run 'docker-compose build' to build, then 'docker-compose up' to run the application. This method is good to test the docker images.</p>
<p>The third way you can run this application is to pull the images from docker hub. No code - just run in the docker image. To do this, visit this repo <a href="https://github.com/dahln/glubfish-deploy">glubfish-deploy</a> at github. This repo contains a docker-compose file which points to the published images in the public repo for this project at docker hub. This method is great for production. On your production machine, 'git clone https://github.com/dahln/glubfish-deploy.git'. After cloned, run 'docker-compose build' or, if you are running in swarm mode (recommended) run this command: 'docker stack deploy -c docker-compose.yml glubfish'. It should come up.</p>
<p>The main "code" project has a file, dockerTagPushToHub.bat. This will push the latest project image to the connected repo. If you use this file, change the image version numbers in there and make them match the version numbers in the docker-compose in the glubfish-deploy project. After you pushing the latest images to docker hub repo, pull the latest docker-compose to your prodcution machine. If you updated the docker-compose to have the latest image version numbers, when should be able to deploye easily.</p>
<h4>Deployment Notes</h4>
<p>NOTE: The docker compose file specifies a storage volume for the mongodb database. That path is for a linux box. Change it for your particular configuration</p>
<p>NOTE: This image is setup with an SSL certificate, associated with the glubfish.dahln.net domain. You can run these images, but you will get a certificate warning. Visit the github for the project source, and modify the images to include your SSL cert.</p>

<h3>Maintenance</h3>
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
  <li>Replaced ngx-modialog with
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