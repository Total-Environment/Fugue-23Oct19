import {connect} from 'react-redux';
import {Checklist} from './components/checklists/checklist';
import axios from 'axios';
import {apiUrl} from '../src/helpers';
import {logException} from "./helpers";
import {logInfo} from "./sumologic-logger";

export const mapStateToProps =
  (state, props) => {
    return ({
      checklistId: props.params.checklistId,
      checklistDetails: state.reducer.currentChecklistDetails,
      error: state.reducer.error
    })
  };
export const mapDispatchToProps = (dispatch) => ({
  onChecklistFetchRequest: async(checklistId) => {
    try {
      const response = await axios.get(apiUrl(`check-lists/${checklistId}`));
      const checklist = response.data;
      const details = checklist.content.entries;
      details.forEach((entry, index) => {
        entry.cells.forEach((cell, cellIndex) => {
          cell.key = index + " " + cellIndex
        });
        entry.key = index;
      });
      logInfo('CHECKLIST_FETCH_SUCCEEDED\n'+ response.data);
      dispatch({type: 'CHECKLIST_FETCH_SUCCEEDED', checklist});
    } catch (error) {
      logException('CHECKLIST_FETCH_FAILED\n'+error);
      if (error.response && error.response.status === 500) {
        dispatch({
          type: 'CHECKLIST_FETCH_FAILED',
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      }
      else if (error.response && error.response.status === 403) {
        dispatch({
          type: 'CHECKLIST_FETCH_FAILED',
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      }
      else if (error.response && error.response.status === 404) {
        dispatch({type: 'CHECKLIST_FETCH_FAILED', error: 'Checklist is not found'});
      }
      else {
        dispatch({type: 'CHECKLIST_FETCH_FAILED', error: error.message});
      }
    }
  },
});

export const ChecklistConnector = connect(mapStateToProps, mapDispatchToProps)(Checklist);
