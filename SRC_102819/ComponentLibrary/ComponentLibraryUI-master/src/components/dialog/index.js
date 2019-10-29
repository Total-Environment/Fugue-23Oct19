import * as React from "react";
import ReactDOM from "react-dom";
import {AlertDialog} from "../alert-dialog";
import {ConfirmationDialog} from "../confirmation-dialog";

const showDialogAsync = (nodeFn) => {
  let removeSelf = x => x;
  const promise = new Promise((resolve, reject) => {
    const container = document.body.appendChild(document.createElement('div'));
    document.getElementById('main').style.filter = 'blur(3px)';
    removeSelf = () => {
      ReactDOM.unmountComponentAtNode(container);
      document.getElementById('main').style.filter = '';
      container.remove();
    };
    ReactDOM.render(nodeFn(resolve, reject), container);
  });
  promise.then(removeSelf, removeSelf);
  return promise;
};

export const alertAsync = (title, message) => {
  return showDialogAsync((resolve, reject) => <AlertDialog onClose={resolve} title={title} message={message} shown />);
};

export const confirmAsync = (title, message) => {
  return showDialogAsync((resolve, reject) => <ConfirmationDialog onYes={resolve} onNo={reject} title={title} message={message} shown />);
};
