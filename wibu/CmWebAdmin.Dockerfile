FROM debian:11.6-slim

LABEL com.wibu.vendor="WIBU-SYSTEMS AG"\
 com.wibu.CodeMeter.Version="7.65.5815"\
 com.wibu.image.authors="info@wibu.com"\
 description="CodeMeter Web Admin Image"\
 author="info@wibu.com"

# Declare com-ports used by CodeMeter WebAdmin in order for http(s) communication
EXPOSE 22352
EXPOSE 22353

# Copy codemeter files into the image
COPY deb/usr/lib/x86_64-linux-gnu/libwibucm.so /usr/lib/x86_64-linux-gnu/libwibucm.so
COPY deb/usr/sbin/CmWebAdmin /usr/sbin/CmWebAdmin

COPY Server.ini /etc/wibu/CodeMeter/Server.ini

# Volume for WebAdmin and config data
VOLUME ["/var/lib/CodeMeter/WebAdmin", "/etc/wibu/CodeMeter"]

ENTRYPOINT ["/usr/sbin/CmWebAdmin"]