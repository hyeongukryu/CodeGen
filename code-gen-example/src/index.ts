import dayjs from 'dayjs';
import { BuiltIns, Organizations, WeatherForecast, type BuiltInsDto } from './api/server';

async function main() {
    await testOrganizations();
    await testWeatherForecast();
    await testBuiltIns();
}
main();

async function testOrganizations() {
    const result = await Organizations.echo({
        a: 9223372036854775807n,
        b: 123456789,
        c: 'TEST🤔',
        d: dayjs('2000-01-01T12:34:56.789Z'),
    });

    assertEqual(result.content, '9223372036854775807\n123456789\nTEST🤔\n946730096789\n',
        'organizations.echo content');
}

async function testWeatherForecast() {
    const result = await WeatherForecast.get(3, 10, 9223372036854775807n);
    assertEqual(result.length, 3, 'weather forecast count');
}

async function testBuiltIns() {
    const request: BuiltInsDto = {
        stringValue: 'TEST🤔',
        booleanValue: true,
        byteValue: 255,
        signedByteValue: -128,
        int16Value: -32768,
        int32Value: -2147483648,
        int64Value: -9223372036854775808n,
        unsignedInt16Value: 65535,
        unsignedInt32Value: 4294967295,
        unsignedInt64Value: 18446744073709551615n,
        singleValue: 123.25,
        doubleValue: -456.5,
        decimalValue: 789.125,
        dateTimeValue: '2024-01-02T03:04:05.006Z',
        dateTimeOffsetValue: '2024-01-02T03:04:05.006+09:00',
        dateOnlyValue: '2024-01-02',
        timeOnlyValue: '03:04:05',
        guidValue: '8f8af7f7-86f1-4a5d-a42c-2e36a12f5f54',
        uriValue: 'https://example.com/path?x=1',
        instantValue: dayjs('2024-01-02T03:04:05.006Z'),
        localDateValue: '2024-01-02',
        localTimeValue: '03:04:05',
        localDateTimeValue: '2024-01-02T03:04:05',
        bytes: new Uint8Array([0, 1, 2, 253, 254, 255]),
    };

    const result = await BuiltIns.echo(request);

    assertEqual(result.stringValue, request.stringValue, 'stringValue');
    assertEqual(result.booleanValue, request.booleanValue, 'booleanValue');
    assertEqual(result.byteValue, request.byteValue, 'byteValue');
    assertEqual(result.signedByteValue, request.signedByteValue, 'signedByteValue');
    assertEqual(result.int16Value, request.int16Value, 'int16Value');
    assertEqual(result.int32Value, request.int32Value, 'int32Value');
    assertEqual(result.int64Value, request.int64Value, 'int64Value');
    assertEqual(result.unsignedInt16Value, request.unsignedInt16Value, 'unsignedInt16Value');
    assertEqual(result.unsignedInt32Value, request.unsignedInt32Value, 'unsignedInt32Value');
    assertEqual(result.unsignedInt64Value, request.unsignedInt64Value, 'unsignedInt64Value');
    assertEqual(result.singleValue, request.singleValue, 'singleValue');
    assertEqual(result.doubleValue, request.doubleValue, 'doubleValue');
    assertEqual(result.decimalValue, request.decimalValue, 'decimalValue');
    assertEqual(result.dateTimeValue, request.dateTimeValue, 'dateTimeValue');
    assertEqual(result.dateTimeOffsetValue, request.dateTimeOffsetValue, 'dateTimeOffsetValue');
    assertEqual(result.dateOnlyValue, request.dateOnlyValue, 'dateOnlyValue');
    assertEqual(result.timeOnlyValue, request.timeOnlyValue, 'timeOnlyValue');
    assertEqual(result.guidValue, request.guidValue, 'guidValue');
    assertEqual(result.uriValue, request.uriValue, 'uriValue');
    assertEqual(result.instantValue.toISOString(), request.instantValue.toISOString(), 'instantValue');
    assertEqual(result.localDateValue, request.localDateValue, 'localDateValue');
    assertEqual(result.localTimeValue, request.localTimeValue, 'localTimeValue');
    assertEqual(result.localDateTimeValue, request.localDateTimeValue, 'localDateTimeValue');
    assertBytesEqual(result.bytes, request.bytes, 'bytes');
}

function assertEqual<T>(actual: T, expected: T, name: string) {
    if (actual !== expected) {
        throw new Error(`Unexpected ${name}: ${String(actual)}`);
    }
}

function assertBytesEqual(actual: Uint8Array, expected: Uint8Array, name: string) {
    if (actual.length !== expected.length) {
        throw new Error(`Unexpected ${name} length: ${actual.length}`);
    }

    for (let i = 0; i < actual.length; i++) {
        if (actual[i] !== expected[i]) {
            throw new Error(`Unexpected ${name}[${i}]: ${actual[i]}`);
        }
    }
}
