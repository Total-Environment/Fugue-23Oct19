import React from 'react';
import styles from './index.css';
import DataType from "../data-types/index";
import { idFor } from "../../helpers";

export const Column = ({ group, componentType, details, onChange }) => {
  let titleElement = null;
  let { editable, isRequired, name, key, dataType, validity, value } = details;
  if (editable && isRequired) {
    titleElement = <span>
      {name}
      <abbr className={styles.requiredMarker} title="Required">*</abbr>
    </span>
  } else {
    titleElement = <span>{name}</span>;
  }
  return <div>
    <dt id={idFor(name, 'title')} className={styles.title}>{titleElement}</dt>
    <dd id={idFor(name, 'value')} className={styles.value}>
      {/* DataTypes are still old format. We have to reuse them till we get rid of Optimus Prime */}
      <DataType columnName={name} columnValue={{ name, key, value, dataType, editable, isRequired, validity }}
        componentType={componentType}
        group={group}
        onChange={onChange} />
    </dd>
  </div>
};
