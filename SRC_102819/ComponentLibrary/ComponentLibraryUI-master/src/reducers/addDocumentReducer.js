const initialState = {
  error: '',
  data: { values: undefined },
  isFetching: false
}

export function addDocumentReducer(state = initialState, action) {
  switch (action.type) {
    case 'GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_STARTED':
      let clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = '';
      clonedState.data = { values: undefined };
      clonedState.isFetching = true;
      return clonedState;
    case 'GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_SUCCEEDED':
      clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = '';
      clonedState.data = { values: action.details };
      clonedState.isFetching = false;
      return clonedState;
    case 'GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_FAILED':
      clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = action.error;
      clonedState.data = { values: undefined };
      clonedState.isFetching = false;
      return clonedState;
    case 'GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_STARTED':
      clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = '';
      clonedState.data = { values: undefined };
      clonedState.isFetching = true;
      return clonedState;
    case 'GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_SUCCEEDED':
      clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = '';
      clonedState.data = { values: action.details };
      clonedState.isFetching = false;
      return clonedState;
    case 'GET_MATERIAL_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_FAILED':
      clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = action.error;
      clonedState.data = { values: undefined };
      clonedState.isFetching = false;
      return clonedState;
    case 'GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_STARTED':
      clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = '';
      clonedState.data = { values: undefined };
      clonedState.isFetching = true;
      return clonedState;
    case 'GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_SUCCEEDED':
      clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = '';
      clonedState.data = { values: action.details };
      clonedState.isFetching = false;
      return clonedState;
    case 'GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_FAILED':
      clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = action.error;
      clonedState.data = { values: undefined };
      clonedState.isFetching = false;
      return clonedState;
    case 'GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_STARTED':
      clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = '';
      clonedState.data = { values: undefined };
      clonedState.isFetching = true;
      return clonedState;
    case 'GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_SUCCEEDED':
      clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = '';
      clonedState.data = { values: action.details };
      clonedState.isFetching = false;
      return clonedState;
    case 'GET_SERVICE_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_FAILED':
      clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = action.error;
      clonedState.data = { values: undefined };
      clonedState.isFetching = false;
      return clonedState;
    case 'GET_BRAND_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_STARTED':
      clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = '';
      clonedState.data = { values: undefined };
      clonedState.isFetching = true;
      return clonedState;
    case 'GET_BRAND_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_SUCCEEDED':
      clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = '';
      clonedState.data = { values: action.details };
      clonedState.isFetching = false;
      return clonedState;
    case 'GET_BRAND_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_FAILED':
      clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = action.error;
      clonedState.data = { values: undefined };
      clonedState.isFetching = false;
      return clonedState;
    case 'GET_BRAND_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_STARTED':
      clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = '';
      clonedState.data = { values: undefined };
      clonedState.isFetching = true;
      return clonedState;
    case 'GET_BRAND_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_SUCCEEDED':
      clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = '';
      clonedState.data = { values: action.details };
      clonedState.isFetching = false;
      return clonedState;
    case 'GET_BRAND_DOCUMENTS_BY_GROUP_AND_COLUMN_NAME_AND_KEYWORD_FAILED':
      clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = action.error;
      clonedState.data = { values: undefined };
      clonedState.isFetching = false;
      return clonedState;
    case 'GET_SFG_DOCUMENTS_BY_GROUP_AND_COLUMN_REQUESTED':
      clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = '';
      clonedState.data = { values: undefined };
      clonedState.isFetching = true;
      return clonedState;
    case 'GET_SFG_DOCUMENTS_BY_GROUP_AND_COLUMN_SUCCEEDED':
      clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = '';
      clonedState.data = { values: action.details };
      clonedState.isFetching = false;
      return clonedState;
    case 'GET_SFG_DOCUMENTS_BY_GROUP_AND_COLUMN_FAILED':
      clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = action.error;
      clonedState.data = { values: undefined };
      clonedState.isFetching = false;
      return clonedState;
    case 'GET_PACKAGE_DOCUMENTS_BY_GROUP_AND_COLUMN_REQUESTED':
      clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = '';
      clonedState.data = { values: undefined };
      clonedState.isFetching = true;
      return clonedState;
    case 'GET_PACKAGE_DOCUMENTS_BY_GROUP_AND_COLUMN_SUCCEEDED':
      clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = '';
      clonedState.data = { values: action.details };
      clonedState.isFetching = false;
      return clonedState;
    case 'GET_PACKAGE_DOCUMENTS_BY_GROUP_AND_COLUMN_FAILED':
      clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = action.error;
      clonedState.data = { values: undefined };
      clonedState.isFetching = false;
      return clonedState;
    case 'DESTROY_MATERIAL_DOCUMENTS':
      clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = '';
      clonedState.data = { values: undefined };
      clonedState.isFetching = false;
      return clonedState;
    case 'DESTROY_SERVICE_DOCUMENTS':
      clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = '';
      clonedState.data = { values: undefined };
      clonedState.isFetching = false;
      return clonedState;
    case 'DESTROY_BRAND_DOCUMENTS':
      clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = '';
      clonedState.data = { values: undefined };
      clonedState.isFetching = false;
      return clonedState;
    case 'DESTROY_SFG_DOCUMENTS':
      clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = '';
      clonedState.data = { values: undefined };
      clonedState.isFetching = false;
      return clonedState;
    case 'DESTROY_PACKAGE_DOCUMENTS':
      clonedState = JSON.parse(JSON.stringify(state));
      clonedState.error = '';
      clonedState.data = { values: undefined };
      clonedState.isFetching = false;
      return clonedState;
    default:
      return state;
  }
}
