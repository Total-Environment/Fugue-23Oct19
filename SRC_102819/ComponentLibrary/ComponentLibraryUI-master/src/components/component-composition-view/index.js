import React, { Component } from 'react';
import { idFor } from "../../helpers";
import styles from './index.css';
import { Link } from "react-router";
import classNames from 'classnames';
import * as R from 'ramda';

export class ComponentCompositionView extends Component {
  render() {
    return <div>
      {this.props.composition.componentCoefficients && this.renderColumnHeaders()}
      {this.props.composition.componentCoefficients.map((header, index) =>
        this.renderComposition(header, index, !this.props.error ? this.props.cost.componentCostBreakup.filter((cost) => cost.componentCode == header.code)[0].cost : ''))}
      {this.props.error ? <span className={styles.error}>{this.props.error}</span> : ''}
    </div>;
  }

  renderCost(cost) {
    return <div id={idFor('value')} className={classNames(styles.columnForComposition, styles.number)}>
      <span className={styles.columnValue}>
        {cost.value || '-'} {cost.currency}
      </span>
    </div>
  }

  renderCompositionColumn(value, number) {
    return <div id={idFor('value')} className={classNames({ [styles.columnForComposition]: true, [styles.number]: number })}>
      <span className={styles.columnValue}>
        {value}
      </span>
    </div>
  }

  renderColumnHeaders() {
    let headers = ["Resource Type", "Component Code", "Component Name", "Unit of Measure", "Coefficient", "Handling/ Storage Wastage %", "Execution Wastage %", "Total Wastage %", "Total Quantity", "Rate"];
    return (<div id={idFor('composition-titles')} className={styles.columnsRow}>
      {headers.map((headerName, i) => this.renderHeaderForComposition(headerName, i > 3))}
    </div>);
  }

  renderComponentCodeForComposition(code, componentType) {
    if (componentType.toLowerCase() === 'asset') {
      return <div className={styles.columnForComposition}>
        <span className={styles.columnValue}>
          <Link to={`/materials/${code}`}>{code}</Link>
        </span>
      </div>
    }
    else if (componentType.toLowerCase() === 'sfg') {
      return <div className={styles.columnForComposition}>
        <span className={styles.columnValue}>
          <Link to={`/sfgs/${code}`}>{code}</Link>
        </span>
      </div>
    }
    return <div className={styles.columnForComposition}>
      <span className={styles.columnValue}>
        <Link to={`/${componentType.toLowerCase()}s/${code}`}>{code}</Link>
      </span>
    </div>
  }

  renderHeaderForComposition(headerName, number) {
    return <div id={idFor(headerName, 'title')} key={headerName} className={classNames(styles.columnForComposition, { [styles.number]: number })}>
      <span className={styles.columnTitleForComposition}>
        {headerName}
      </span>
    </div>
  }

  renderWastagePercentages(wastagePercentages) {
    if (R.isEmpty(wastagePercentages)) {
      wastagePercentages = [0, 0].map(c => ({ value: c }));
    }
    if (R.length(wastagePercentages) <= 1) {
      let name = R.head(wastagePercentages).name;
      wastagePercentages = R.contains('handling', name.toLowerCase()) ?
        R.append({ value: 0 }, wastagePercentages) : R.prepend({ value: 0 }, wastagePercentages);
    }
    return wastagePercentages.map(wastagePercentage => this.renderCompositionColumn(wastagePercentage.value, true));
  }

  renderComposition(header, index, cost) {
    return (<div id={idFor('composition-values', index)} className={styles.columnsRow}>
      {this.renderCompositionColumn(header.componentType)}
      {this.renderComponentCodeForComposition(header.code, header.componentType)}
      {this.renderCompositionColumn(header.name)}
      {this.renderCompositionColumn(header.unitOfMeasure)}
      {this.renderCompositionColumn(header.coefficient, true)}
      {this.renderWastagePercentages(header.wastagePercentages)}
      {this.renderCompositionColumn(header.totalWastagePercentage, true)}
      {this.renderCompositionColumn(header.totalQuantity, true)}
      {this.renderCost(cost)}
    </div>);
  }
}
