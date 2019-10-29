import React, { PropTypes } from 'react';
import { Link } from 'react-router';
import { brand, addBrand } from './index.css';
import { button } from '../../../css-common/forms.css';

export const Brand = (props, context) => {
  if (props.columnValue.editable) return <div />;
  const brandCode = props.columnValue.value.columns.find(({ key }) => key === 'brand_code').value;
  return <div className={brand}><a
    href={`/${context.componentType}s/${context.componentCode}/brands/${brandCode}`}>
    {brandCode}
  </a></div>;
};

export const AddBrand = (props, context) => (props.mode === 'edit' || props.mode === 'create') ? <div></div> : <div className={brand}>
  <a className={[button, addBrand].join(' ')} href={`/${context.componentType}s/${context.componentCode}/brands/new`}>
    {"Add New"}
  </a>
</div>

Brand.contextTypes = {
  componentCode: PropTypes.string,
  componentType: PropTypes.string,
};

AddBrand.contextTypes = {
  componentCode: PropTypes.string,
  componentType: PropTypes.string,
};
