#!/usr/bin/env bash

pnpm build && cp out/bundle.js ../code-gen-web/src/bundle.js
