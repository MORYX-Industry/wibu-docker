# Wibu Docker Environment

This is a summarized instruction to create and setup a docker environment for applications that depend on CodeMeter from Wibu Systems. 
The official instructions from Wibu can be found in the pdf [CodeMeter Developer – CodeMeter in Docker](https://www.wibu.com/support/manuals-guides.html).

## Prerequisites

Before you begin, ensure you have the following tools? installed and configured:

1. **Docker**: Make sure Docker is installed on your system. You can download it from the [official Docker website](https://www.docker.com/get-started).
2. **Docker Compose**: Docker Compose should also be installed. You can follow the installation instructions on the [Docker Compose documentation](https://docs.docker.com/compose/install/).
3. **WSL (if not already on Linux)**: If you haven't set up WSL, you can follow the guide on the [Microsoft documentation](https://docs.microsoft.com/en-us/windows/wsl/install).

\*I think we can assume that readers know what WSL is or would be able to understand when following the link.

> **Note**: All commands in this tutorial were tested on WSL using Ubuntu.


## Update the CodeMeter dependencies

To build the CodeMeter container we need a set of libraries and files from the CodeMeter Runtime for Linux.
In order to start off with the latest version or to update it later on, you need to get the necessary files from the Wibu Website.
The following set of instructions downloads and unpacks the files to copy them into the right directories.

Download the .deb file

    wget -O wibu.deb "https://www.wibu.com/support/user/user-software/file/download/14039.html?tx_wibudownloads_downloadlist%5BdirectDownload%5D=directDownload&tx_wibudownloads_downloadlist%5BuseAwsS3%5D=0&cHash=8dba7ab094dec6267346f04fce2a2bcd"

Make sure to replace the direct link in the following instructions with the direct link to the latest version of the [CodeMeter User Runtime for Linux](https://www.wibu.com/support/user/user-software.html) 
(We need the Driver-Only Linux 64-bit DEB Package).

\*Maybe biased, but I'm not a fan of this "We"-phrasing and would stick to either "we" or "you" or use some 'generic' phrases.

```
# Create a directory to unpack the .deb file \* can't dpkg do that? i.e. doesn't it have a target dir option?
mkdir -p deb

# Unpack the .deb file into the deb directory
dpkg-deb -x wibu.deb deb

# Copy the relevant files replacing the old version of CodeMeter
cp -r deb/usr/bin/ wibu/deb/usr/
cp -r deb/usr/sbin/ wibu/deb/usr/
cp -r deb/usr/lib/ wibu/deb/usr/
```
\*not sure if we should have only the commands as code and the comments as text instead of comments

If everything worked out you are ready to go and can build the CodeMeter container with the latest version of the CodeMeter Runtime.


## Setup the CodeMeter environment

### Start the docker containers

Make sure docker is running and bring up the containers from the compose file.
```
docker compose up -d 
```
Now, you should be able to access the WebAdmin tool at [localhost:22080](http://localhost:22080/dashboard.html).


### Setup licenses from license files

Currently the CodeMeter instance does not provide any licenses. 
Opening the [license overview](http://localhost:22080/license_monitoring.html) should prompt you a warning telling you this.
You'll find an example license file here: [Moryx.Launcher.WibuCmRaU](wibu\licenses\Moryx.Launcher.WibuCmRaU) \* maybe absolute url
\* If this is going to be a blog post, I would refrain from talking about 'this repo'
The folder is mapped to the CodeMeter container, which allows you to add the license running the command below.

```
docker exec CmLicenseService cmu -i -f /.wibu/Moryx.Launcher.WibuCmRaU
```

By refreshing the WebAdmin page, you should now see the first license in you docker container.

\*Instead of the brakets below I would add an info section here like 'Container vs Container' to explain the difference
and also always highlight the specific type in the text. E.g.: docker container only (not CodeMeter docker container) and
Wibu License Container. Maybe introduce an abbreviation in that explanation and only use WLC from there on.

Beware, for a license to be activatable in a CodeMeter docker container the license-container needs to allow this.
For our demo licenses, we set the -lopt configuration in the .wbc file. (\* We)
The file can be found [here](wibu\licenses\Moryx.Launcher.wbc) (\* location) as a reference example. 
For licenses that are provided by the License Central, this configuration must be done in there.

\*for names like "License Central", "CodeMeter", etc. I'm not sure if we have to use the official names and case including "WiBu" ("WIBU"?)
and trademarks.


### Setup a corresponding Network License Server

\* You should announce those two options at the beginning of the chapter

Alternatively, you can decide to run the CodeMeter docker container purely as a proxy to hand licenses from a Network License Server to your application docker? containers.
This can even be a CodeMeter instance on your docker host, if you don't want allow? your licenses to be activated in the docker conatiner directly.
> **Note**: In this case you have to make sure, however, that using network licenses is enabled when encrypting your code and that you provide network licenses to your users. When using a `.WibuCpsConf` file with the AxProtector, the configuration to look out for is `SubSystem: LocalLan`*`

For a full set of instructions on how to setup CodeMeter, please refer to the official instructions and Help material from Wibu. 
This short paragraph is intended to quickly bring you up to speed with the basic configurations necessary.


#### Installation

To setup a second CodeMeter Runtime to act as the network license server follow either of the follwoing tutorials to install the CodeMeter Runtime first

- *Linux*: https://wiki.tuflow.com/Installing_Wibu_CodeMeter_Linux
- *Windows*: Just run the [CodeMeter User Runtime for Windows installer](https://www.wibu.com/support/user/user-software.html) or use the [Phoenix Contact ActivationWizard](https://www.phoenixcontact.com/en-pc/products/software-plcnext-engineer-1046008)


#### Server Setup

Now you have to make sure, that the CodeMeter instance is enabled to act as a network license server. For that,

- open the WebAdmin tool of your server instance 
- go to `Configuration > Server > Server Access`
- check the *Enable for Network Server* checkbox
- apply the changes and restart the CodeMeter Runtime


#### Client Setup

When you installed the network license server on your docker host you are good to go, as your codemeter container will find that server automatically.
If not, open the WebAdmin page from the docker container and 

- go to `Configuration > Basic > Server Search List`
- add the server to the Server Search List and apply the change


## Bring it to the server

Assuming you don't want to build your docker images on the system you later want to run them on, here is the list of docker command for your convenience


#### Manually copying local images to the server

Create tar files for your WIBU images

    sudo docker save -o ./docker-images/CmLicenseServer.tar wibu/codemeter:base 
    sudo docker save -o ./docker-images/CmWebAdmin.tar wibu/codemeter:webadmin 

Copy to the server into the `docker-images` directory using WinSCP or similar.

    sudo docker load -i ./docker-images/CmLicenseServer.tar // Only required for initial setup
    sudo docker load -i ./docker-images/CmWebAdmin.tar // Only required for initial setup

Check if everything worked out using 

    docker images


If you want to remove the old images execute

    docker image prune


#### Push the imags to a docker registry of your choice

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