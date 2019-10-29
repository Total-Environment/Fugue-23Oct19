import axios from 'axios';
import * as R from 'ramda';
import { apiUrl, sasContainer } from './helpers';

const cacheAuthDetails = (authData) => {
  sasContainer.tokenHash = R.map((sasUrl) => {
    const searchParams = (new URL(sasUrl)).searchParams;
    searchParams.set(authData.cdnToken.paramName, authData.cdnToken.token);
    return searchParams;
  }, authData.sasTokens);
};
/**
 * Get SAS token which is valid for specified duration
 * @param {int} duration
 */
export function updateSasToken(duration) {
  return axios.get(apiUrl(`auth/CL?duration=${duration}`), { headers: { Accept: 'application/json' } })
    .then(R.prop('data'))
    .then(cacheAuthDetails)
    .then(() => {
      // Retry after specified duration
      setTimeout(() => updateSasToken(duration), duration * (60-10) * 1000);
    });
}
