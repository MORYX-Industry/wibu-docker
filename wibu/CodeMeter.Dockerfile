FROM debian:11.6-slim

LABEL com.wibu.vendor="WIBU-SYSTEMS AG"\
 com.wibu.CodeMeter.Version="7.65.5815"\
 com.wibu.image.authors="info@wibu.com"\
 description="CodeMeter Base Image"\
 author="info@wibu.com"

# Port used by CodeMeter - see Server.ini#NetworkPort
EXPOSE 22350

# Copy codemeter files into the image
COPY deb/usr/bin /usr/bin
COPY deb/usr/sbin /usr/sbin
COPY deb/usr/lib /usr/lib
COPY deb/var /var

# Prepared Server.ini with all configs set beforehand.
COPY Server.ini /etc/wibu/CodeMeter/Server.ini

# CodeMeterLin -x will return "3" which fails the build step of run since it's not equal to 0
RUN apt update && apt install -y libusb-1.0-0 procps && /usr/sbin/CodeMeterLin -x || exit 0

# Volume for logging and config data
VOLUME ["/var/log/CodeMeter", "/etc/wibu/CodeMeter"]

# Ensure a safe shutdown of the CodeMeter Service
STOPSIGNAL SIGINT
ENTRYPOINT ["/usr/sbin/CodeMeterLin", "-v"]

# Enable logging and put CodeMeterLin into forground mode as init process
CMD ["-l+"]