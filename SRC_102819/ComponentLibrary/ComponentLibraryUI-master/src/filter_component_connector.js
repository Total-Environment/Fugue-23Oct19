import axios from 'axios';
import { apiUrl, logException } from './helpers';
import { connect } from 'react-redux';
import { FilterComponent } from '../src/components/filter-component';
import {logInfo} from "./sumologic-logger";

export const mapStateToProps = (state, props) => ({
  definitions: state.reducer.definitions || {},
  brandDefinitions: state.reducer.brandDefinitions || {},
  dependencyDefinitions: state.reducer.dependencyDefinitions,
  group: props.group,
  onClose: props.onClose,
  componentType: props.componentType,
  onApply: props.onApply,
  onClear: props.onClear,
  errorInFilter: props.errorInFilter,
  modifiedDefinition: props.modifiedDefinition,
  onCancelErrorDialog: props.onCancelErrorDialog,
  levels: props.levels
});

export const mapDispatchToProps = (dispatch) => ({
  onMaterialDefinitionFetch: async (group) => {
    try {
      const response = await axios.get(apiUrl(`material-definitions/${group}`));
      logInfo('COMPONENT_MATERIAL_DEFINITION_FETCH_SUCCEEDED \n' + response.data);
      dispatch({ type: 'COMPONENT_MATERIAL_DEFINITION_FETCH_SUCCEEDED', definition: response.data });
    }
    catch (error) {
      logException('COMPONENT_MATERIAL_DEFINITION_FETCH_FAILED \n ' + error);
      dispatch({ type: 'COMPONENT_MATERIAL_DEFINITION_FETCH_FAILED', error });
    }
  },
  onBrandDefinitionFetch: async () => {
    try {
      const response = await axios.get(apiUrl(`definitions/brands`));
      logInfo('COMPONENT_BRAND_DEFINITION_FETCH_SUCCEEDED \n' + response.data);
      dispatch({ type: 'COMPONENT_BRAND_DEFINITION_FETCH_SUCCEEDED', brandDefinition: response.data });
    }
    catch (error) {
      logException('COMPONENT_BRAND_DEFINITION_FETCH_FAILED \n' + error);
      dispatch({ type: 'COMPONENT_BRAND_DEFINITION_FETCH_FAILED', error });
    }
  },

  onServiceDefinitionFetch: async (group) => {
    try {
      const response = await axios.get(apiUrl(`service-definitions/${group}`));
      logInfo('COMPONENT_SERVICE_DEFINITION_FETCH_SUCCEEDED \n '+ response.data);
      dispatch({ type: 'COMPONENT_SERVICE_DEFINITION_FETCH_SUCCEEDED', definition: response.data });
    }
    catch (error) {
      logException('COMPONENT_SERVICE_DEFINITION_FETCH_FAILED \n ' + error);
      dispatch({ type: 'COMPONENT_SERVICE_DEFINITION_FETCH_FAILED', error });
    }
  },

  onSFGDefinitionFetch: async () => {
    try {
      const response = await axios.get(apiUrl(`sfg-definitions`));
      logInfo('SFG_DEFINITION_FETCH_SUCCEEDED \n' + response.data);
      dispatch({ type: 'SFG_DEFINITION_FETCH_SUCCEEDED', definition: response.data });
    }
    catch (error) {
      logException('SFG_DEFINITION_FETCH_FAILED \n' + error);
      dispatch({ type: 'SFG_DEFINITION_FETCH_FAILED', error });
    }
  },

  onPackageDefinitionFetch: async () => {
    try {
      const response = await axios.get(apiUrl(`package-definitions`));
      logInfo('PACKAGE_DEFINITION_FETCH_SUCCEEDED \n' + response.data);
      dispatch({ type: 'PACKAGE_DEFINITION_FETCH_SUCCEEDED', definition: response.data });
    }
    catch (error) {
      logException('PACKAGE_DEFINITION_FETCH_FAILED \n' +error);
      dispatch({ type: 'PACKAGE_DEFINITION_FETCH_FAILED', error });
    }
  },

  onDependencyDefinitionFetch: async (dependencyDefinitionId) => {
    try {
      const response = await axios.get(apiUrl(`dependency/${dependencyDefinitionId}?compressed=true`));
      const dependencyDefinition = response.data;
      logInfo('DEPENDENCY_DEFINITION_FETCH_SUCCEEDED \n' + response.data);
      dispatch({type: 'DEPENDENCY_DEFINITION_FETCH_SUCCEEDED', dependencyDefinitionId, dependencyDefinition});
    }
    catch (error) {
      logException('DEPENDENCY_DEFINITION_FETCH_FAILED \n' + error);
      if (error.response && error.response.status === 500) {
        dispatch({
          type: 'DEPENDENCY_DEFINITION_FETCH_FAILED',
          dependencyDefinitionId,
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      }
      else if (error.response && error.response.status === 403) {
        dispatch({
          type: 'DEPENDENCY_DEFINITION_FETCH_FAILED',
          dependencyDefinitionId,
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      }
      else if (error.response && error.response.status === 404) {
        dispatch({
          type: 'DEPENDENCY_DEFINITION_FETCH_FAILED',
          dependencyDefinitionId,
          error: `dependency definition: ${dependencyDefinitionId} is not found`
        });
      }
      else {
        dispatch({type: 'DEPENDENCY_DEFINITION_FETCH_FAILED', dependencyDefinitionId, error});
      }
    }
  },
});

export const FilterComponentConnector = connect(mapStateToProps, mapDispatchToProps)(FilterComponent);
