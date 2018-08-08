# dotnet-encrypt

Encrypt a file that can be read using `ChatLe.Cryptography.Utility`.

## Install

``` bash
dotnet tool install -g dotnet-encrypt
```

## Usage

### Encrypt

In a command promp type:

    encrypt {path to the file to encryp} {secret-key}

expemple: `encrypt filetosecure.json "My secure key"`

The tool generate an encrypted file beside de file to encrypt with the same name but with *.enc* extension.  
In our sample the encrypted file is named *filetosecure.json.enc*.

### Decrypt

This file can decrypted by `ChatLe.Cryptography.Utility`.  

Install the nuget package `ChatLe.Cryptography`  
Wrote a bit of code to decryp the file:

    using (var utility = new Utility("My secure key")
    {
        using (var stream = await utility.DecryptFile("filetosecure.json.enc"))
        {
            // code using the decrypted stream
        }
    }
