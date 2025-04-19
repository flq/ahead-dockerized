# Ahead.Dockerized

I am one of the lead developers who develop ahead intranet, [the digital home](https://aheadintranet.com) for countless employees mostly across the DACH (Germany, Austrie, Switzerland) region.

This repository holds the code relevant to the blog series [starting here](https://realfiction.net/posts/depending-on-a-color/) on my website.

The basic premise is this: 

> Considering the architecture of the product ahead, which services would one use, and how, to reduce dependencies on Azure PaaS services
to the point where one could host the product outside of Azure?

## info on connecting to the graph db via gremlin console

You can use the `tinkerpop/gremlin-console:latest` container as such:

```sh
docker run --rm -it \                                                                                                                    ─╯
-v "$(pwd)/remote.yaml:/opt/gremlin-console/conf/remote.yaml" \
tinkerpop/gremlin-console
```

Assuming you're in the data directory of the solution

Once in the terminal you connect and then run scripts remotely via

```sh
:remote connect conf/remote.yaml
:remote console
```

`:remote close` closes the connection again