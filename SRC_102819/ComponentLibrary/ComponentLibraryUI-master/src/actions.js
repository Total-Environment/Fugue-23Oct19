import axios from 'axios';
import {logException, apiUrl, needsFetching} from "./helpers";
import {alertAsync} from "./components/dialog/index";

export const masterDataFetch = (id) => async (dispatch) => {
  try {
    dispatch({type: 'COMPONENT_MASTER_DATA_FETCH_LOADING', masterDataId: id});
    const response = await axios.get(apiUrl(`master-data/${id}`));
    const masterData = response.data;
    dispatch({type: 'COMPONENT_MASTER_DATA_FETCH_SUCCEEDED', masterData});
  } catch (error) {
    logException(error);
    dispatch({type: 'COMPONENT_MASTER_DATA_FETCH_FAILED', error});
  }
};

export const onSfgFetchRequest = (sfgCode) => async (dispatch) => {
  try {
    dispatch({type: 'SFG_FETCH_STARTED', code: sfgCode});
    const response = await axios.get(apiUrl(`sfgs/${sfgCode}`));
    dispatch({type: 'SFG_FETCH_SUCCEEDED', details: response.data});
  }
  catch (error) {
    logException(error);
    if (error.response && error.response.status === 404) {
      dispatch({
        type: 'SFG_FETCH_FAILED',
        code: sfgCode,
        error: {type: 'NotFound', message: `SFG ${sfgCode} is not available in the database`}
      });
    }
    else {
      dispatch({
        type: 'SFG_FETCH_FAILED',
        code: sfgCode,
        error: {type: 'Unknown', message: 'Some thing went wrong. Problem has been recorded and will be fixed.'}
      });
    }
  }
};

export const fetchMasterDataByName = name => async (dispatch) => {
  try {
    dispatch({type: 'MASTER_DATA_BY_NAME_FETCH_REQUESTED', name});
    const response = await axios.get(apiUrl(`master-data/name/${name}`));
    const masterData = response.data;
    dispatch({type: 'MASTER_DATA_BY_NAME_FETCH_SUCCEEDED', masterData, name});
  } catch (error) {
    logException(error);
    //alertAsync('Error', (error && error.message) || error);
    if (error.response && error.response.status === 404) {
      dispatch({type: 'MASTER_DATA_BY_NAME_FETCH_FAILED',name, error: `master data for "${name}" is NotFound`});
    } else if (error.response && error.response.status === 400) {
      dispatch({type: 'MASTER_DATA_BY_NAME_FETCH_FAILED', name,error: `unable to get master data for "${name}"`});
    }
    else {
      let errorMessage = error.message;
      if(error.message && error.message.toLowerCase() === "network error") {
        errorMessage = "Component Library server is down. Please reach out to admin.";
        dispatch({type: 'MASTER_DATA_BY_NAME_FETCH_FAILED',name, error: errorMessage});
      }
      else {
        dispatch({type: 'MASTER_DATA_BY_NAME_FETCH_FAILED',
          name,
          error: (error.response && error.response.data && error.response.data.message) || errorMessage});
      }
    }
  }
};

export const fetchDependencyDefinition = componentType => async (dispatch) => {
  let dependencyDefinitionId = `${componentType}Classifications`;
  try {
    dispatch({
      type: 'DEPENDENCY_DEFINITION_FETCH_REQUESTED',
      dependencyDefinitionId
    });
    const response = await axios.get(apiUrl(`dependency/${dependencyDefinitionId}?compressed=true`));
    const dependencyDefinition = response.data;
    dispatch({
      type: 'DEPENDENCY_DEFINITION_FETCH_SUCCEEDED',
      dependencyDefinitionId,
      dependencyDefinition
    });
  }
  catch (error) {
    logException(error);
    //alertAsync('Error', `Failed to fetch ${componentType} classification.`);
    let errorMessage = error.message;
    if(error.message && error.message.toLowerCase() === "network error") {
      errorMessage = "Component Library server is down. Please reach out to admin.";
      dispatch({type: 'DEPENDENCY_DEFINITION_FETCH_FAILED',dependencyDefinitionId,componentType, error: errorMessage});
    }
    else
    {
      dispatch({
        type: 'DEPENDENCY_DEFINITION_FETCH_FAILED',
        dependencyDefinitionId,
        componentType,
        error: (error.response && error.response.data && error.response.data.message) || errorMessage
      });
    }
  }
};
export const fetchDependencyDefinitionIfNeeded = componentType => async (dispatch, getState) =>
needsFetching(getState().reducer.dependencyDefinitions[`${componentType.toLowerCase()}Classifications`]) && dispatch(fetchDependencyDefinition(componentType.toLowerCase()));

export const fetchMasterDataByNameIfNeeded = name => async (dispatch, getState) =>
needsFetching(getState().reducer.masterDataByName[name]) && dispatch(fetchMasterDataByName(name));

export const onPackageFetchRequest = (packageCode) => async (dispatch) => {
  try {
    dispatch({type: 'PACKAGE_FETCH_STARTED', code: packageCode});
    const response = await axios.get(apiUrl(`packages/${packageCode}`));
    dispatch({type: 'PACKAGE_FETCH_SUCCEEDED', details: response.data});
  }
  catch (error) {
    logException(error);
    if (error.response && error.response.status === 404) {
      dispatch({
        type: 'PACKAGE_FETCH_FAILED',
        code: packageCode,
        error: {
          type: 'NotFound',
          message: `Package ${packageCode} is not available in the database`
        }
      });
    }
    else {
      dispatch({
        type: 'PACKAGE_FETCH_FAILED',
        code: packageCode,
        error: {type: 'Unknown', message: 'Some thing went wrong. Problem has been recorded and will be fixed.'}
      });
    }
  }
};

export const fetchCprs = (filters) => async (dispatch) => {
  dispatch({type: 'FETCH_CPRS_REQUESTED'});
  try {
    const response = await axios.get(apiUrl('costpriceratios'), {params: filters});
    dispatch({type: 'FETCH_CPRS_SUCCEEDED', cprs: response.data});
  } catch(error) {
    logException(error);
    let errorMessage = error.message;
    if(error.message && error.message.toLowerCase() === "network error") {
      errorMessage = "Component Library server is down. Please reach out to admin.";
      dispatch({type: 'FETCH_CPRS_FAILED', error: errorMessage});
    }
    else
    {
      dispatch({
        type: 'FETCH_CPRS_FAILED',
        error: (error.response && error.response.data && error.response.data.message) || errorMessage
      });
    }
  }
};

export const fetchProjects = () => async(dispatch) => {
  dispatch({type: 'FETCH_PROJECTS_REQUESTED'});
  try {
    const response = await axios.get(apiUrl('projects'));
    dispatch({type: 'FETCH_PROJECTS_SUCCEEDED', projects: response.data});
  } catch(error) {
    logException(error);
    let errorMessage = error.message;
    if(error.message && error.message.toLowerCase() === "network error") {
      errorMessage = "Component Library server is down. Please reach out to admin.";
      dispatch({type: 'FETCH_PROJECTS_FAILED', error: errorMessage});
    }
    else
    {
      dispatch({
        type: 'FETCH_PROJECTS_FAILED',
        error: (error.response && error.response.data && error.response.data.message) || errorMessage
      });
    }
  }
};
export const fetchProjectsIfNeeded = () => async (dispatch, getState) =>
  needsFetching(getState().reducer.projects) && dispatch(fetchProjects());
