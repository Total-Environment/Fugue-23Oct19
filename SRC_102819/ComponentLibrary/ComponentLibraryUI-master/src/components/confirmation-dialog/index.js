import React from 'react';
import Modal from 'react-modal';
import styles from './index.css';
import { Icon } from '../icon';
import {idFor} from '../../helpers';

export const ConfirmationDialog = (props) => <Modal className={styles.dialog} isOpen={props.shown} contentLabel="Confirmation">
  <Icon name="squirrel" className={styles.icon} />
  <div className={styles.content}>
    <h3>{props.title}</h3>
    <p>{props.message}</p>
  </div>
  <button id={idFor('yes')} className={styles.button} onClick={props.onYes}>Yes</button>
  <button id={idFor('no')} className={styles.button} onClick={props.onNo}>No</button>
</Modal>;
