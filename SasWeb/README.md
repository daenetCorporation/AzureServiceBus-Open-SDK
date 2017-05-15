Markup :  # Shared access signatures (SAS): #

A shared access signature (SAS) provides you with a way to grant limited access to objects in your storage account to other clients, without exposing your account key.

What is a shared access signature?

A SAS is a secure way to share your storage resources because it provides limited permissions to your storage account in regarding to clients, who do not have the account key.

When should you use a shared access signature?

You can use a SAS when you want to provide access to resources in your storage account to any client not possessing your storage account's access keys. Your storage account includes both a primary and secondary access key, both of which grant administrative access to your account, and all resources within it. Exposing either of these keys opens your account to the possibility of malicious or negligent use. Shared access signatures provide a safe alternative that allows clients to read, write, and delete data in your storage account according to the permissions you've explicitly granted, and without need for an account key.

How does SAS works

A shared access signature is a signed URI that points to one or more storage resources and includes a token that contains a special set of query parameters. The token determines how client is being accessed.. One of the query parameters, the signature, is constructed from the SAS parameters and signed with the account key. This signature is used by Azure Storage to authenticate the SAS.

