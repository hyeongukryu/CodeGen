import typescript from '@rollup/plugin-typescript';
import terser from '@rollup/plugin-terser';
import { nodeResolve } from '@rollup/plugin-node-resolve';
import commonjs from '@rollup/plugin-commonjs';
import json from '@rollup/plugin-json';

export default {
    input: 'index.ts',
    output: {
        file: 'out/bundle.js',
        format: 'cjs'
    },
    plugins: [
        json(),
        commonjs(),
        nodeResolve(),
        typescript(),
        terser(),
    ],
};
