import http from 'k6/http';
import { check, sleep } from 'k6';

const baseUrl = (__ENV.BASE_URL || '').replace(/\/$/, '');
if (!baseUrl) {
  throw new Error('Set BASE_URL to the deployed test site URL.');
}

export const options = {
  vus: 5,
  duration: '30s',
  thresholds: {
    http_req_failed: ['rate<0.01'],
    http_req_duration: ['p(95)<2000'],
  },
};

export default function () {
  const response = http.get(`${baseUrl}/Donation/Create`);
  check(response, {
    'donation page returns HTTP 200': (result) => result.status === 200,
    'donation form is rendered': (result) => result.body.includes('Make a Donation'),
  });
  sleep(1);
}
