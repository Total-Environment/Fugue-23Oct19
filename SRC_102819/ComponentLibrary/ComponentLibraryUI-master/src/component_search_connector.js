import {connect} from 'react-redux';
import {ComponentSearch} from './components/component-search';
import {apiUrl} from '../src/helpers';
import axios from 'axios';
import {browserHistory} from 'react-router';
import querystring from 'querystring';
import componentDataTransformer from '../src/middlewares/component';
import {logException} from "./helpers";
import {logInfo} from "./sumologic-logger";

export const mapStateToProps = (state, props) => {
  if (!props.isListing) {
    return {
      componentType: {material: 'material', service: 'service', sfg: 'SFG', package: 'package'}[props.route.componentType],
      search: state.reducer.search,
      group: props.params.group,
      keyword: props.params.keyword,
      error: state.reducer.error,
      sortOrder: props.location.query.sortOrder,
      sortColumn: props.location.query.sortColumn,
      selectedTab: props.location.query.selectedTab,
      pageNumber: +(props.location.query.pageNumber || 1),
      isFiltering: state.reducer.isFiltering || false,
      filterData:state.reducer.filterData || []
    }
  }
  else return {
    componentType: props.componentType,
    search: state.reducer.search,
    group: "",
    keyword: "",
    error: state.reducer.error
  }
};

const getParams = (group, keyword, pageNumber, isListing, sortColumn, sortOrder,filterDatas) => {
  const batchSize = 20;
  if (isListing) {
    return {pageNumber, details: true, batchSize};
  } else {
    return {searchQuery: keyword, pageNumber, groupname: group,batchSize, details: true, sortColumn, sortOrder,filterDatas};
  }
};

const getParamsWithoutGroup = (keyword, pageNumber, isListing, sortColumn, sortOrder,filterDatas) => {
  const batchSize = 20;
  if (isListing) {
    return {pageNumber, details: true, batchSize};
  } else {
    return {searchQuery: keyword, pageNumber, batchSize, details: true, sortColumn, sortOrder,filterDatas};
  }
};

export const mapDispatchToProps = (dispatch, ownProps) => ({
  onMaterialSearchResultsFetch: async(group, keyword, pageNumber, isListing, sortColumn, sortOrder, filterDatas) => {
    try {
      dispatch({type: 'SEARCH_RESULT_FETCH_STARTED', isFiltering: filterDatas ? !!filterDatas.length : false, filterData:filterDatas});
      let response;
      if (isListing) {
        response = await axios.get(apiUrl('materials/all'), {params: getParams(group, keyword, pageNumber, isListing, sortColumn, sortOrder, filterDatas)});
      } else {
        response = await axios.post(apiUrl('materials/searchwithingroup'), getParams(group, keyword, pageNumber, isListing, sortColumn, sortOrder,filterDatas));
      }
      const responseJson = response.data;
      logInfo('SEARCH_RESULT_FETCH_SUCCEEDED \n'+response.data);
      dispatch({type: 'SEARCH_RESULT_FETCH_SUCCEEDED', details: responseJson});
    }
    catch (error) {
      logException('SEARCH_RESULT_FETCH_FAILED \n' +error);
      if (error.response && error.response.status === 404) {
        dispatch({type: 'SEARCH_RESULT_FETCH_FAILED', error: {type: 'NotFound'}});
      } else if (error.response && error.response.status === 400) {
        dispatch({type: 'SEARCH_RESULT_FETCH_FAILED', error: {type: 'BadRequest'}});
      }
      else {
        let errorMessage = error.message;
        if(error.message && error.message.toLowerCase() === "network error") {
          errorMessage = "Component Library server is down. Please reach out to admin.";
          dispatch({type: 'SEARCH_RESULT_FETCH_FAILED', error: {type: 'NetworkError', error: errorMessage}});
        }
        else {
          dispatch({type: 'SEARCH_RESULT_FETCH_FAILED',
            error: {
              type: 'Unknown',
              error: (error.response && error.response.data && error.response.data.message) || errorMessage
            }
          });
        }
      }
    }
  },
  onMaterialSearchDestroy: () => {
    dispatch({type: 'SEARCH_RESULT_DESTROYED'});
  },
  onServiceSearchResultsFetch: async(group, keyword, pageNumber, isListing, sortColumn, sortOrder,filterDatas) => {
    try {
      dispatch({type: 'SEARCH_RESULT_DESTROYED'});
      dispatch({type: 'SEARCH_RESULT_FETCH_STARTED', isFiltering: filterDatas ? !!filterDatas.length : false, filterData: filterDatas});
      let response;
      if (isListing) {
        response = await axios.get(apiUrl('services'), {params: getParams(group, keyword, pageNumber, isListing, sortColumn, sortOrder, filterDatas)});
      }else{
        response = await axios.post(apiUrl('services/searchwithingroup'), getParams(group, keyword, pageNumber, isListing, sortColumn, sortOrder,filterDatas));
      }
      const responseJson = response.data;
      logInfo('SEARCH_RESULT_FETCH_SUCCEEDED\n' +response.data);
      dispatch({type: 'SEARCH_RESULT_FETCH_SUCCEEDED', details: responseJson});
    }
    catch (error) {
      logException('SEARCH_RESULT_FETCH_FAILED\n' +error);
      if (error.response && error.response.status === 404) {
        dispatch({type: 'SEARCH_RESULT_FETCH_FAILED', error: {type: 'NotFound'}});
      } else if (error.response && error.response.status === 400) {
        dispatch({type: 'SEARCH_RESULT_FETCH_FAILED', error: {type: 'BadRequest'}});
      }
      else {
        let errorMessage = error.message;
        if(error.message && error.message.toLowerCase() === "network error") {
          errorMessage = "Component Library server is down. Please reach out to admin.";
          dispatch({type: 'SEARCH_RESULT_FETCH_FAILED', error: {type: 'NetworkError', error: errorMessage}});
        }
        else {
          dispatch({type: 'SEARCH_RESULT_FETCH_FAILED',
            error: {
              type: 'Unknown',
              error: (error.response && error.response.data && error.response.data.message) || errorMessage
            }
          });
        }
      }
    }
  },
  onServiceSearchDestroy: () => {
    dispatch({type: 'SEARCH_RESULT_DESTROYED'});
  },
  onSfgSearchResultsFetch: async(keyword, pageNumber, isListing, sortColumn, sortOrder,filterDatas) => {
    try {
      dispatch({type: 'SEARCH_RESULT_FETCH_STARTED', isFiltering: filterDatas ? !!filterDatas.length : false, filterData:filterDatas});
      let response;
      if (isListing) {
        response = await axios.get(apiUrl('sfgs/all'), {params: getParamsWithoutGroup(keyword, pageNumber, isListing, sortColumn, sortOrder, filterDatas)});
      } else {
        response = await axios.post(apiUrl('sfgs/search'), getParamsWithoutGroup(keyword, pageNumber, isListing, sortColumn, sortOrder,filterDatas));
      }
      const responseJson = response.data;
      logInfo('SEARCH_RESULT_FETCH_SUCCEEDED\n'+response.data);
      dispatch({type: 'SEARCH_RESULT_FETCH_SUCCEEDED', details: responseJson});
    }
    catch (error) {
      logException('SEARCH_RESULT_FETCH_FAILED\n'+error);
      if (error.response && error.response.status === 404) {
        dispatch({type: 'SEARCH_RESULT_FETCH_FAILED', error: {type: 'NotFound'}});
      } else if (error.response && error.response.status === 400) {
        dispatch({type: 'SEARCH_RESULT_FETCH_FAILED', error: {type: 'BadRequest'}});
      }
      else {
        let errorMessage = error.message;
        if(error.message && error.message.toLowerCase() === "network error") {
          errorMessage = "Component Library server is down. Please reach out to admin.";
          dispatch({type: 'SEARCH_RESULT_FETCH_FAILED', error: {type: 'NetworkError', error: errorMessage}});
        }
        else {
          dispatch({type: 'SEARCH_RESULT_FETCH_FAILED',
            error: {
              type: 'Unknown',
              error: (error.response && error.response.data && error.response.data.message) || errorMessage
            }
          });
        }
      }
    }
  },
  onSfgSearchDestroy: () => {
    dispatch({type: 'SEARCH_RESULT_DESTROYED'});
  },
  onPackageSearchResultsFetch: async(keyword, pageNumber, isListing, sortColumn, sortOrder, filterDatas) => {
    try {
      dispatch({type: 'SEARCH_RESULT_FETCH_STARTED', isFiltering: filterDatas ? !!filterDatas.length : false, filterData:filterDatas});
      let response;
      if (isListing) {
        response = await axios.get(apiUrl('packages/all'), {params: getParamsWithoutGroup(keyword, pageNumber, isListing, sortColumn, sortOrder, filterDatas)});
      } else {
        response = await axios.post(apiUrl('packages/search'), getParamsWithoutGroup(keyword, pageNumber, isListing, sortColumn, sortOrder,filterDatas));
      }
      const responseJson = response.data;
      logInfo('SEARCH_RESULT_FETCH_SUCCEEDED\n'+response.data);
      dispatch({type: 'SEARCH_RESULT_FETCH_SUCCEEDED', details: responseJson});
    }
    catch (error) {
      logException('SEARCH_RESULT_FETCH_FAILED\n'+error);
      if (error.response && error.response.status === 404) {
        dispatch({type: 'SEARCH_RESULT_FETCH_FAILED', error: {type: 'NotFound'}});
      } else if (error.response && error.response.status === 400) {
        dispatch({type: 'SEARCH_RESULT_FETCH_FAILED', error: {type: 'BadRequest'}});
      }
      else {
        let errorMessage = error.message;
        if(error.message && error.message.toLowerCase() === "network error") {
          errorMessage = "Component Library server is down. Please reach out to admin.";
          dispatch({type: 'SEARCH_RESULT_FETCH_FAILED', error: {type: 'NetworkError', error: errorMessage}});
        }
        else {
          dispatch({type: 'SEARCH_RESULT_FETCH_FAILED',
            error: {
              type: 'Unknown',
              error: (error.response && error.response.data && error.response.data.message) || errorMessage
            }
          });
        }
      }
    }
  },
  onPackageSearchDestroy: () => {
    dispatch({type: 'SEARCH_RESULT_DESTROYED'});
  },
  onAmendQueryParams: (params) => {
    const newParams = Object.assign({}, ownProps.location.query, params);
    if(ownProps.route.componentType.toLowerCase() === 'sfg')
      browserHistory.push(`/sfgs/search/${ownProps.params.keyword}?${querystring.stringify(newParams)}`);
    else if(ownProps.route.componentType.toLowerCase() === 'package')
      browserHistory.push(`/packages/search/${ownProps.params.keyword}?${querystring.stringify(newParams)}`);
    else {
      const endpoint = {
        'material': 'materials',
        'service': 'services'
      }[ownProps.route.componentType.toLowerCase()];
      browserHistory.push(`/${endpoint}/search/${ownProps.params.group}/${ownProps.params.keyword}?${querystring.stringify(newParams)}`);
    }
  }
});

export const ComponentSearchConnector = connect(mapStateToProps, mapDispatchToProps)(ComponentSearch);
