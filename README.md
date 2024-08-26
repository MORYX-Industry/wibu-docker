# MORYX Docker Environment
**In addition to the instructions on the main branch, we include an example moryx application and instructions to setup yours in a docker environment**

This is a summarized instruction to create and setup a docker environment for applications that depend on CodeMeter from Wibu Systems. 
The official instructions from Wibu can be found in the pdf [CodeMeter Developer – CodeMeter in Docker](https://www.wibu.com/support/manuals-guides.html).

## Prerequisites

Before you begin, ensure you have the following installed and configured:

1. **Docker**: Make sure Docker is installed on your system. You can download it from the [official Docker website](https://www.docker.com/get-started).
2. **Docker Compose**: Docker Compose should also be installed. You can follow the installation instructions on the [Docker Compose documentation](https://docs.docker.com/compose/install/).
3. (If you are not running Linux) **WSL (Windows Subsystem for Linux)**: If you haven't set up WSL, you can follow the guide on the [Microsoft documentation](https://docs.microsoft.com/en-us/windows/wsl/install).

> **Note**: All commands in this tutorial were tested on WSL using Ubuntu.


## Update the CodeMeter dependencies
To build the CodeMeter container we need a set of libraries and files from the CodeMeter Runtime for Linux.
In order to start of with the latest version or to update it later on, you need to get the necessary files from the Wibu Website.
The following set of instructions downloads and unpacks the files to copy them into the right directories.
Make sure to replace the direct link in the following instructions with the direct link to the latest version of the [CodeMeter User Runtime for Linux](https://www.wibu.com/support/user/user-software.html) (We need the Driver-Only Linux 64-bit DEB Package).

```
# Download the .deb file
wget -O wibu.deb "https://www.wibu.com/support/user/user-software/file/download/14039.html?tx_wibudownloads_downloadlist%5BdirectDownload%5D=directDownload&tx_wibudownloads_downloadlist%5BuseAwsS3%5D=0&cHash=8dba7ab094dec6267346f04fce2a2bcd"

# Create a directory to unpack the .deb file
mkdir -p deb

# Unpack the .deb file into the deb directory
dpkg-deb -x wibu.deb deb

# Copy the relevant files replacing old version of CodeMeter
cp -r deb/usr/bin/ wibu/deb/usr/
cp -r deb/usr/sbin/ wibu/deb/usr/
cp -r deb/usr/lib/ wibu/deb/usr/

```
If everything worked out you are ready to go and can build the CodeMeter container with the latest version of the CodeMeter Runtime.

## Setup the CodeMeter environment
### Start the docker containers

Make sure docker is running and bring up the containers from the compose file.
```
docker compose up -d 
```
You should be able now to open the WebAdmin tool under [localhost:22080](http://localhost:22080/dashboard.html).


### Setup licenses from license files
Currently the CodeMeter instance does not provide any licenses. 
Opening the [license overview](http://localhost:22080/license_monitoring.html) should prompt you a warning telling you this.
We have included an example license file [Moryx.Launcher.WibuCmRaU](wibu\licenses\Moryx.Launcher.WibuCmRaU) in this repository. 
The folder is mapped to the CodeMeter container, which allows you to add the license running the command below.
```
docker exec CmLicenseService cmu -i -f /.wibu/Moryx.Launcher.WibuCmRaU
```
When you refresh the WebAdmin page, you should now see the first license in you docker container.

Beware, for a license to be activatable in a CodeMeter docker container the license-container (confusing I know... I don't mean the docker container here but the thing that holds licenses in Wibu, which is also called a container) needs to allow this.
For our demo licenses, we set the -lopt configuration in the .wbc file. 
The file can be found [here](wibu\licenses\Moryx.Launcher.wbc) as a reference example. 
For licenses that are provided by the License Central, this configuration must be done in there.

### Setup a corresponding Network License Server

Alternatively, you can decide to run the CodeMeter docker container purely as a proxy to hand licenses from a Network License Server to your application containers.
This can even be a CodeMeter instance on your docker host, if you don't want to enable your licenses to be activated in the docker conatiner directly.
>*Note: In this case you have to make sure, however, that using network licenses is enable when encrypting your code and that you provide network licenses to your users. When using a .WibuCpsConf file with the AxProtector the configuration to look out for is `SubSystem: LocalLan`*`

For a full set of instruction how to setup CodeMeter, please refer to the official instructions and Help material from Wibu. 
This short paragraph is intended to quickly bring you up to speed with the basic configurations necessary.

#### Installation
To setup a second CodeMeter Runtime to act as the network license server follow either of the follwoing tutorials to install the CodeMeter Runtime first
- **Linux**: https://wiki.tuflow.com/Installing_Wibu_CodeMeter_Linux
- **Windows**: Just run the [CodeMeter User Runtime for Windows](https://www.wibu.com/support/user/user-software.html) installer or use the [ActivationWizard](https://www.phoenixcontact.com/en-pc/products/software-plcnext-engineer-1046008) from Phoenix Contact

#### Server Setup
Now you need to make sure, that the CodeMeter instance is enabled to act as a network license server. For that,
- Open the WebAdmin tool of your server instance 
- Go to Configuration > Server > Server Access
- Check the Enable for Network Server checkbox
- Apply the changes and restart the CodeMeter Runtime

#### Client Setup
When you installed the network license server on your docker host you are good to go, as your codemeter container will find that server automatically.
If not, open the WebAdmin page from the docker container and 
- Go to Configuration > Basic > Server Search List
- Add the server to the Server Search List and apply the change


## Bring it to the server
Assuming you don't want to build your docker images on the system you later want to run them on, here is the list of docker command for your convenience

#### Manually copying local images to the server
Create tar files for your wibu images
```
sudo docker save -o ./docker-images/CmLicenseServer.tar wibu/codemeter:base 
sudo docker save -o ./docker-images/CmWebAdmin.tar wibu/codemeter:webadmin 
```
Copy to the server into the `docker-images` directory using WinSCP or similar.
```
sudo docker load -i ./docker-images/CmLicenseServer.tar // Only required for initial setup
sudo docker load -i ./docker-images/CmWebAdmin.tar // Only required for initial setup
```
Check if everything worked out using 
```
docker images
```

If you want to remove the old images execute
```
docker image prune
```

#### Push the images to a docker registry of your choice
```
# Build the Docker images:
docker compose build

# Tag the Docker images
docker tag wibu/codemeter:base <registry_url>/<repository>/<codemeter_image_name>:<tag>
docker tag wibu/codemeter:webadmin <registry_url>/<repository>/<webadmin_image_name>:<tag>

# Log in to the Docker registry
docker login <registry_url>

# Push the Docker images to the registry
docker push <registry_url>/<repository>/<codemeter_image_name>:<tag>
docker push <registry_url>/<repository>/<webadmin_image_name>:<tag>
```

## Add a MORYX application
If you already have a MORYX application you can of course also manage it in a seperate repo. 
#### Initialize using the CLI
To wrap up this tutorial we, however, we start by adding a new application using the moryx CLI
```
moryx new Example --no-git-init
```
#### Add docker support to the application project
Now we add docker support to our application project. 
If you are working with Visual Studio, there is a [convenient option from the context menu](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/visual-studio-tools-for-docker?view=aspnetcore-8.0#existing-app).
Whether or not you used this option you now need to add or extend the Dockerfile in the *src/Example.App* directory to match [this one](src/Example.App/Dockerfile).
Compared to the standard Dockerfile that Visual Studio would have added we made two changes here.
First, we add the `Directory.Build.props`, `Directory.Build.targets`, and `NuGet.Config` files to ensure that the build process inside the Docker container uses the same configuration and dependencies as our local development environment.
```
...
# Add these lines to the Dockerfile before the dotnet restore command
COPY ["Directory.Build.props", "."]
COPY ["Directory.Build.targets", "."]
COPY ["NuGet.Config", "."]
...
```
Second, we need to add the Wibu dependencies to our final image to be able to use the encrypted MORYX packages.
```
# Add these lines to the part creating the final image
COPY wibu/deb/usr/lib/x86_64-linux-gnu/libwibucm.so /usr/lib/x86_64-linux-gnu/libwibucm.so
COPY wibu/deb/usr/lib/x86_64-linux-gnu/libcpsrt.so /usr/lib/x86_64-linux-gnu/libcpsrt.so
```
Lastly, we are going to adjust the `compose.yml` file.
Where up till now only a placeholder indicated how to include your app into the Wibu architecture, we will now put the required instructions to build and use our app instead.
```yml
moryx:
    build: # Build the docker image from the code
      context: ./
      dockerfile: ./src/Example.App/Dockerfile
    image: moryx/example:latest
    container_name: moryx
    restart: unless-stopped
    ports: # Adjust the mapped ports to match your use case
      -8080:8080
      -8081:8081
    environment: # We need to set this env variable for the Wibu dependencies to work
      CODEMETER_HOST: codemeter_license_service
    volumes: # Persiting the configs and logs 
      - moryx-config:/app/Config
      - moryx-logs:/app/Logs
    networks: # In addition to the default network we need to put our app into the network of the codemeter container
      - default
      - codemeter-network
    depends_on: # Add a dependency on the codemeter docker container
      - codemeter_license_service
```
That's it, when you [bring up your composed docker infrastructure](#start-the-docker-containers) with the full [compose.yml](compose.yml) file your application is ready to go!
