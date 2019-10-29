import * as React from "react";
import {Loading} from "./components/loading";
import {connect} from "react-redux";

const renderLoading = message => <div style={{
  position: 'fixed',
  top: 0,
  left: 0,
  width: '100%',
  zIndex: 10000,
  background: 'rgba(0,0,0,0.3)',
  height: '100%'
}}>
  <Loading />
  <p style={{textAlign: 'center',fontStyle: 'italic', color: '#fff'}}>{message}</p>
</div>;

const FullLoading = ({status, message}) => (status ? renderLoading(message) : <div/>);

export const FullLoadingConnector = connect(state => state.reducer.loading)(FullLoading);
