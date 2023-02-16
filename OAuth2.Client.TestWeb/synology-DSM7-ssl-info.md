# When connect to providers on Synology NAS width DSM7 throws exception:

1. Try use legacy TLS ```export MONO_TLS_PROVIDER=legacy && echo $MONO_TLS_PROVIDER```
1. Sync root CA for root ```sudo /volume1/@appstore/mono/bin/cert-sync /etc/ssl/certs/ca-certificates.crt```
1. Sync root CA for current user ```/volume1/@appstore/mono/bin/cert-sync --user /etc/ssl/certs/ca-certificates.crt```
1. Run server ```mono OAuth2.Client.TestWeb.exe --environment=synology```