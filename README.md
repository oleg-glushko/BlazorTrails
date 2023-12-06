The completed [Blazor in Action](https://www.manning.com/books/blazor-in-action) project with a more self-hosted and FOSS approach in mind. Instead of Auth0, it uses Keycloak (see instructions below), placeholder.com dummy image is saved and placed at noimage.png, SixLabors.ImageSharp library changed to Magick.NET. Due to the Leaflet author's dick move, where he added a malicious code to his library, I made a local copy of it and removed all this nonsense. It's better to avoid such libs and tools in production altogether, there is a nice alternative - [openlayers](https://github.com/openlayers/openlayers).

Create an initial database by applying migrations and run the API project, which hosts a Blazor client (pre .NET 8 approach). Open it by the https://localhost:7288/ URL and log in as a "blazingtrails" realm's user (see instructions below).

```shell
dotnet ef database update -p BlazingTrails.Api
dotnet run --lp https --project BlazingTrails.Api
```

# TLS certificate
It's possible to run Keycloak without TLS, and this could be a tempting shortcut for a lab. However, I try to avoid developing bad habits and use a self-signed certificate to mimic best practices. The easiest way to obtain a certificate for local experiments is to use a self-signed one. A dotnet tool already gives you a certificate through its dev-certs command. Run the command below to export it in the PEM format.

```shell
dotnet dev-certs https -ep .\keys\keycloak.crt -np --format pem
```

Note: if you can't or don't want to import this self-signed certificate into the System's Trusted Issuers, you must create a custom validator. Below is an example of a validator that gets a certificate from the User's Trusted Issuers storage. The logic is naive, and it skips check on cert chains, so don't use it in production.

```csharp
.AddJwtBearer(options => options.BackchannelHttpHandler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (sender, cert, chain, errors) =>
    {
        if (cert is null || errors != System.Net.Security.SslPolicyErrors.None)
            return false;

        using var store = new X509Store(StoreName.Root, StoreLocation.CurrentUser, OpenFlags.ReadOnly);
        var certs = store.Certificates.Find(X509FindType.FindByThumbprint, cert.Thumbprint, false);
        if (certs.Count == 0)
            return false;

        foreach (var localCert in certs)
        {
            if (localCert.GetCertHashString() == cert.GetCertHashString())
                return cert.NotAfter > DateTime.UtcNow;
        }

        return false;
    }
});
```

# Keycloak configuration

The easiest way to run Keycloak is by using a container technology like [Podman](https://github.com/containers/podman) or [Rancher Desktop](https://github.com/rancher-sandbox/rancher-desktop/releases). Don't forget to mount a directory with the previously acquired key pair and tell Keycloak to use it. Take note that the command below is for a Linux shell. Windows requires backslashes in paths, and you need to replace `$(pwd)` with `\` and remove all line wraps / trailing slashes.

```shell
docker run -d --name keycloak -p 8443:8443 \
--mount type=bind,source=$(pwd)/keys,target=/keys \
-e KEYCLOAK_ADMIN=admin -e KEYCLOAK_ADMIN_PASSWORD=admin \
-e KC_HTTPS_CERTIFICATE_FILE=/keys/keycloak.crt \
-e KC_HTTPS_CERTIFICATE_KEY_FILE=/keys/keycloak.key \
quay.io/keycloak/keycloak:22.0 \
start-dev
```

Now it's time to configure Keycloak (see also the [official docs](https://www.keycloak.org/docs/latest/server_admin/)).

1. Go to https://localhost:8433/admin.
2. Login using the "admin:admin" requisites.
3. In the top left dropdown menu, click on the "Create Realm" button.
4. Set its name to "blazingtrails" and finish the wizard.
5. Move to the "Clients" tab of the menu at left.
6. Create a new client, set its Client ID to "https://blazingtrailsapi.com" and name to "Blazing Trails API". You can turn off all authentication flows on the next page - this client is used only as a so-called "Valid Audience".
7. Next, create another client using these settings.
   - The Client ID "blazing-trails-client" and name "Blazing Trails Client"
   - Leave active the "Standard flow"
   - Valid redirect URI "https://localhost:7288/authentication/login-callback"
   - Valid post logout redirect URI "https://localhost:7288/authentication/logout-callback"
   - Web origin "https://localhost:7288"
8. Move to the "Roles" tab of this client.
9. Create a new role and name it "Administrator".
10. Move to the "Client scopes" tab and the "roles" scope's "Assigned type" to "Optional" 
11. Click the "blazing-trails-client-dedicated" link. You're now on its subtab "Mappers".
12. Add a new mapper "by configuration" using the "Audience" type using these settings:
    - Name it "https://blazingtrailsapi.com"
    - Select the previously created client with the same name
    - Make sure that the "Add to access token" flag is active
13. Create another mapper of the type "User Attribute"
    - Name it "NameAsEmail" - Keycloak uses a username for the "name" claim by default, but the app requires it to be an email
    - Type "email" in "User Attribute," and enter "name" as "Token Claim Name"
    - "Claim JSON Type" should be "String" and mapper applies to ID and Access tokens
14. Create another mapper of the type "User Client Role". Keycloak puts roles in the "realm_access" claim (we disabled it at step 10), but the app expects them to be in the "role" claim.
    - Name it "Roles" and select the "blazing-trails-client" as "Client ID"
    - Multivalued flag must be active
    - Set "Token Claim Name" to "role" and "Claim JSON Type" to "String"
    - This mapper should apply both to ID and Access tokens
15. Move from the "Mappers" subtab to the "Scope" of the "blazing-trails-client-dedicated" scopes. Uncheck the "Full scope allowed" flag to turn off adding all the realm roles.
16. Select "Users" from the left menu and click the "Add User" button.
    - Input some username and email, and put a flag on "Email verified"
    - Finish the user creation master, then go to its "Credentials" tab and set a password (uncheck the "Temporary" flag)
    - If this user must have full access to the app, go to the "Role mappings" tab and add it to the "Administrator" role of the "blazing-trail-client" (select "Filter by clients" in a dropdown).
17. If you wish, you may increase sessions' (and tokens') longevity at the "Sessions" tab of "Realm settings".
